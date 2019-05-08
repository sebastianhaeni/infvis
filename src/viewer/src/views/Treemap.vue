<template>
    <section>
        <output>&nbsp;{{timestamp}}</output>
        <div id="chart-controls">
            <input type="button" value="Play" @click="startPlaying()" v-if="!isPlaying">
            <input type="button" value="Pause" @click="pausePlaying()" v-if="isPlaying">
            <input type="range"
                   class="timeslice-range"
                   min="0"
                   value="0"
                   max="100"
                   step="1"
                   v-model="timesliceProgress"
                   @change="updateTimeslice()"
                   @input="updateTimestamp()">
        </div>
        <div id="chart"></div>
    </section>
</template>

<script lang="ts">
    import {Component, Vue} from 'vue-property-decorator';
    import {treemap} from './treemap.support';

    declare const d3: any;

    @Component
    export default class Treemap extends Vue {

        public timestamp = 'loading...';
        private data: { EndTime: string, Elements: { [index: string]: any } }[] = [];
        private timesliceProgress: number = 0;
        private isPlaying = false;
        private domain: string[] = [];

        startPlaying() {
            this.isPlaying = true;
            this.advanceTime();
        }

        advanceTime() {
            if (!this.isPlaying) {
                return;
            }
            this.timesliceProgress = (this.timesliceProgress += 1) % 100;
            this.updateTimestamp();
            this.draw();
            setTimeout(() => this.advanceTime(), 800);
        }

        pausePlaying() {
            this.isPlaying = false;
        }

        updateTimeslice() {
            this.draw()
        }

        updateTimestamp() {
            const end = this.getDataIndex();
            this.timestamp = this.data[end - 1].EndTime;
        }

        mounted() {
            d3.json(process.env.VUE_APP_DATA_URL, (err, res) => {
                if (err) {
                    return;
                }

                this.data = res;

                this.domain = this.data
                    .flatMap(d => Object.values(d.Elements))
                    .map(val => val.FilePath.split('/').flatMap(p => p.split('.'))[0])
                    .filter((value, index, self) => self.indexOf(value) === index)
                    .filter(Boolean)
                    .sort();

                this.updateTimestamp();
                this.draw();
            });
        }

        private draw() {
            this.$el.children[this.$el.children.length - 1].innerHTML = '';

            const end = this.getDataIndex();

            const all = this.data.slice(0, end)
                .map(d => d.Elements)
                .reduce((a, b, i) => {
                    // aggregate time slices and remove deleted files
                    const resetDelta = i + 1 < end;
                    Object.entries(b).forEach(entry => {
                        if (entry[1].LinesOfCode === 0) {
                            // file deleted => we delete it from our map
                            delete a[entry[0]];
                        } else {
                            if (resetDelta) {
                                // the current slice is not relevant for delta lines
                                entry[1].RelativeLinesDelta = 0;
                            }
                            a[entry[0]] = entry[1];
                        }
                    });
                    return a;
                }, {});

            const mappedResponse = Object.keys(all)
                .map(path => {
                    // map to intermediary tree map structure
                    let parts = path.split('/').flatMap(p => p.split(/\.(?!cs$)/));
                    const region = parts[0];
                    const subregion = parts[1];
                    const key = parts[2];
                    return {
                        key: key || subregion || region,
                        region: region,
                        subregion: subregion || region,
                        value: all[path].LinesOfCode,
                        delta: Math.abs(all[path].RelativeLinesDelta),
                    };
                })
                .reduce((acc, current) => {
                    // aggregate everything with depth > 3
                    const existing = acc.find(val => {
                        return val.key === current.key && val.region === current.region && val.subregion === current.subregion;
                    });
                    if (existing) {
                        existing.value += current.value;
                        existing.delta += current.delta
                    } else {
                        acc.push(current);
                    }
                    return acc;
                }, []);

            const data = d3.nest()
                .key(d => d.region)
                .key(d => d.subregion)
                .entries(mappedResponse);

            treemap({}, {key: "Root", values: data}, this.domain);
        }

        private getDataIndex() {
            return Math.max(Math.round((this.data.length / 100) * this.timesliceProgress), 1);
        }

        /*private doNest() {
                    this.nest = d3.nest();

                    for (let i = 0; i < this.depth; i++) {
                        this.nest = this.nest.key(d => d.dir[i]);
                    }

                    this.nest.rollup(d => ({
                        total: d3.sum(d, d => d.LinesOfCode),
                        change: d3.sum(d, d => Math.abs(d.RelativeLinesDelta)),
                    }));
                }

                private draw() {
                    this.$el.children[this.$el.children.length - 1].innerHTML = '';
                    const end = Math.max(Math.round((this.data.length / 100) * this.timesliceProgress), 1);
                    this.timestamp = this.data[end - 1].EndTime;

                    const all = this.data.slice(0, end)
                        .map(d => d.Elements)
                        .reduce((a, b, i) => {
                            const resetDelta = i + 1 < end;
                            Object.entries(b).forEach(entry => {
                                if (entry[1].LinesOfCode === 0) {
                                    // file deleted => we delete it from our map
                                    delete a[entry[0]];
                                } else {
                                    if (resetDelta) {
                                        // the current slice is not relevant for delta lines
                                        entry[1].RelativeLinesDelta = 0;
                                    }
                                    a[entry[0]] = entry[1];
                                }
                            });
                            return a;
                        }, {});

                    const timeslice = Object.values(all).map(entry => ({
                        dir: entry.FilePath.replace('.cs', '').split(/[\/.]/),
                        ...entry
                    }));

                    const values = this.nest.entries(timeslice);
                    const root = d3.hierarchy({values}, d => d.values)
                        .sum((d: any) => d.value ? d.value.total : 0)
                        .sort((a, b) => b.value - a.value);

                    this.treemap(root);

                    const topPadding = 130;

                    // add clusters
                    const cluster = d3.select(this.$el.children[this.$el.children.length - 1])
                        .selectAll('.cluster')
                        .data(root.children)
                        .enter()
                        .append('div')
                        .attr('class', 'cluster')
                        .style("left", (d: any) => d.x0 + "px")
                        .style("top", (d: any) => d.y0 + topPadding + "px")
                        .style("width", (d: any) => d.x1 - d.x0 + "px")
                        .style("height", (d: any) => d.y1 - d.y0 + "px")
                        .attr('title', d => this.getNodeText(d as any));

                    const node = cluster.selectAll('.node')
                        .data((d: any) => d._children || [d])
                        .enter()
                        .append("div")
                        .attr("class", "node")
                        .style("left", (d: any) => d.x0 + "px")
                        .style("top", (d: any) => d.y0 + topPadding + "px")
                        .style("width", (d: any) => d.x1 - d.x0 + "px")
                        .style("height", (d: any) => d.y1 - d.y0 + "px")
                        .attr('title', d => this.getNodeText(d as any))
                        .style("background-color", d => this.getRGBABackground(d as any));

                    // add node
                    /!* const node = d3.select(this.$el.children[this.$el.children.length - 1])
                         .selectAll(".node")
                         .data(root.leaves())
                         .enter()
                         .append("div")
                         .attr("class", "node")
                         .style("left", (d: any) => d.x0 + "px")
                         .style("top", (d: any) => d.y0 + topPadding + "px")
                         .style("width", (d: any) => d.x1 - d.x0 + "px")
                         .style("height", (d: any) => d.y1 - d.y0 + "px")
                         .attr('title', d => this.getNodeText(d as any))
                         .style("background-color", d => this.getRGBABackground(d as any));*!/

                    // add label
                    node.append("div")
                        .attr("class", "node-label")
                        .text(d => this.getNodeText(d as any));

                    if (!this.debug) {
                        return;
                    }

                    // add value
                    node.append("div")
                        .attr("class", "node-value")
                        .text(d => `Total lines: ${this.format(d.value)}`);

                    // add delta
                    node.append("div")
                        .attr("class", "node-delta")
                        .text((d: any) => `Lines changed: ${d.data.value.change}`);
                }

                private getRGBABackground(d: HierarchyNode<{ value: { change: number } }>): string {
                    const alpha = Math.min(d.data.value.change, 1000) / 1000;
                    return 'rgba(255, 0, 0, ' + alpha + ')';
                }*/

    }
</script>

<style lang="scss">
    #chart {
        background: #fff;
        font-family: "Helvetica Neue", Helvetica, Arial, sans-serif;
        min-height: calc(100vh - 129px)
    }

    .title {
        font-weight: bold;
        font-size: 24px;
        text-align: center;
        margin-top: 6px;
        margin-bottom: 6px;
    }

    text {
        pointer-events: none;
    }

    .grandparent text {
        font-weight: bold;
    }

    rect {
        fill: none;
        stroke: #fff;
    }

    rect.parent,
    .grandparent rect {
        stroke-width: 2px;
    }

    rect.parent {
        pointer-events: none;
    }

    .grandparent rect {
        fill: orange;
    }

    .grandparent:hover rect {
        fill: #ee9700;
    }

    .children rect.parent,
    .grandparent rect {
        cursor: pointer;
    }

    .children rect.parent {
        fill: #bbb;
        fill-opacity: .2;
    }

    .children:hover rect.child {
        fill: #bbb;
    }

    .timeslice-range {
        width: calc(100vw - 30px);
    }
</style>

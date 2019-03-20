<template>
    <section>
        <div id="chart-controls">
            <input type="button" value="Play" @click="play()">
            <input type="range"
                   class="timeslice-range"
                   min="0"
                   value="0"
                   max="100"
                   step="1"
                   v-model="timesliceProgress"
                   @change="updateTimeslice()">
        </div>
        <div id="chart"></div>
    </section>
</template>

<script lang="ts">
    import {Component, Vue} from 'vue-property-decorator';
    import * as d3 from "d3";
    import {HierarchyNode} from "d3";

    type Datum = {
        FilePath: string,
        AbsoluteLinesDelta: number,
        LinesAdded: number,
        LinesDeleted: number,
        LinesOfCode: number,
        RelativeLinesDelta: number,

        parent?: Datum,
        _children?: Datum[],
    };

    @Component
    export default class Treemap extends Vue {

        private data: { Elements: { [index: string]: Datum } }[] = [];
        private timesliceProgress: number = 0;
        private nest;
        private treemap;
        private format = d3.formatLocale({
            decimal: ".",
            thousands: ",",
            grouping: [3],
            currency: ["CHF", ""]
        }).format("d");

        play() {
            this.timesliceProgress = (this.timesliceProgress += 1) % 100;
            this.draw();
            setTimeout(() => this.play(), 200);
        }

        updateTimeslice() {
            this.draw()
        }

        mounted() {
            const depth = 5;

            const width = this.$el.clientWidth;
            const height = this.$el.clientHeight;

            this.nest = d3.nest();

            for (let i = 0; i < depth; i++) {
                this.nest = this.nest.key(d => d.dir[i]);
            }

            this.nest.rollup(d => ({
                total: d3.sum(d, d => d.LinesOfCode),
                change: d3.sum(d, d => d.RelativeLinesDelta),
            }));

            this.treemap = d3.treemap()
                .size([width, height])
                .padding(1)
                .round(true);

            d3.json("git-aggregate.json").then((data: { Elements: { [index: string]: Datum } }[]) => {
                this.data = data;
                this.draw();
            });
        }

        private draw() {
            this.$el.children[this.$el.children.length - 1].innerHTML = '';
            const end = Math.max(Math.round((this.data.length / 100) * this.timesliceProgress), 1);

            const all = this.data.slice(0, end)
                .map(d => d.Elements)
                .reduce((a, b) => {
                    Object.entries(b).forEach(entry => {
                        if (entry[1].LinesOfCode === 0) {
                            // file deleted => we delete it from our map
                            delete a[entry[0]];
                        } else {
                            a[entry[0]] = entry[1];
                        }
                    });
                    return a;
                }, {});

            const timeslice = Object.values(all).map(entry => ({
                dir: entry.FilePath.split('/'),
                ...entry
            }));

            let values = this.nest.entries(timeslice);
            const root = d3.hierarchy({values}, d => d.values)
                .sum(d => d.value ? d.value.total : 0)
                .sort((a, b) => b.value - a.value);

            this.treemap(root);

            const topPadding = 130;

            // add node
            const node = d3.select(this.$el.children[this.$el.children.length - 1])
                .selectAll(".node")
                .data(root.leaves())
                .enter()
                .append("div")
                .attr("class", "node")
                .style("left", d => d.x0 + "px")
                .style("top", d => d.y0 + topPadding + "px")
                .style("width", d => d.x1 - d.x0 + "px")
                .style("height", d => d.y1 - d.y0 + "px")
                .attr('title', d => this.getNodeText(d))
                .style("background-color", d => this.getRGBABackground(d));

            // add label
            node.append("div")
                .attr("class", "node-label")
                .text(d => this.getNodeText(d));

            // add value
            node.append("div")
                .attr("class", "node-value")
                .text(d => this.format(d.value));
        }

        private getRGBABackground(d: HierarchyNode): string {
            const alpha = Math.min(d.data.value.change, 100) / 100;
            return 'rgba(255, 0, 0, ' + alpha + ')';
        }

        private getNodeText(d: HierarchyNode): string {
            let text = d.data.key && d.data.key !== 'undefined' ? d.data.key : '';
            while (d.parent && d.parent.data.key) {
                if (d.parent.data.key !== 'undefined') {
                    let separator = text.length > 0 ? '/' : '';
                    text = d.parent.data.key + separator + text;
                }
                d = d.parent;
            }
            return text;
        }
    }
</script>

<style lang="scss">

    #chart {
        min-height: calc(100vh - (2 * 30px));
        min-width: 100%;
    }

    .node {
        box-sizing: border-box;
        line-height: 1em;
        overflow: hidden;
        position: absolute;
        white-space: pre;
        background: #ddd;

        &-label,
        &-value {
            margin: 4px;
        }

        &-value {
            margin-top: -2px;
            font-weight: bold;
        }
    }

    .timeslice-range {
        width: calc(100vw - 30px);
    }
</style>

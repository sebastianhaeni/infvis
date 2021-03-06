<template>
    <section>
        <output>{{timestamp}}</output>
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
                        const file = {...entry[1]};
                        if (file.LinesOfCode === 0) {
                            // file deleted => we delete it from our map
                            delete a[entry[0]];
                        } else {
                            if (resetDelta) {
                                // the current slice is not relevant for delta lines
                                file.RelativeLinesDelta = 0;
                            }
                            a[entry[0]] = file;
                        }
                    });
                    return a;
                }, {});

            const mappedResponse = Object.keys(all)
                .map(path => {
                    // map to intermediary tree map structure
                    let parts = path.split('/').flatMap(p => p.split(/\.(?![a-z]+$)/));
                    return {
                        key: parts[parts.length - 1],
                        parts: parts,
                        value: all[path].LinesOfCode,
                        delta: Math.abs(all[path].RelativeLinesDelta),
                    };
                });

            let data = d3.nest();

            for(let i = 0; i < 10; i++) {
                data = data.key(d => d.parts[i]);
            }

            data = data.entries(mappedResponse);

            treemap({}, {key: "Root", values: data}, this.domain);
        }

        private getDataIndex() {
            return Math.max(Math.round((this.data.length / 100) * this.timesliceProgress), 1);
        }
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

    .tooltipster-base {
        font-family: "Helvetica Neue", Helvetica, Arial, sans-serif;
    }
</style>

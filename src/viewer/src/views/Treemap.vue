<template>
    <div id="chart"></div>
</template>

<script lang="ts">
    import {Component, Vue} from 'vue-property-decorator';
    import * as d3 from "d3";

    type Datum = {
        key: string,
        region: string,
        subregion: string,
        value: number,

        parent?: Datum,
        _children?: Datum[],
    };

    @Component
    export default class Treemap extends Vue {
        mounted() {

            const depth = 5;

            const width = this.$el.clientWidth;
            const height = this.$el.clientHeight;

            const format = d3.formatLocale({
                decimal: ".",
                thousands: ",",
                grouping: [3],
                currency: ["CHF", ""]
            }).format("d");

            let nest = d3.nest();

            for (let i = 0; i < depth; i++) {
                nest = nest.key(d => d.dir[i]);
            }

            nest.rollup(d => d3.sum(d, d => d.LinesOfCode));

            const treemap = d3.treemap()
                .size([width, height])
                .padding(1)
                .round(true);

            d3.json("git-aggregate.json").then((data) => {
                let timeframe = Object.values(data[0].Elements)
                    .map(entry => ({
                        dir: entry.FilePath.split('/'),
                        ...entry
                    }));
                console.log(timeframe);

                const root = d3.hierarchy({values: nest.entries(timeframe)}, d => d.values)
                    .sum(d => d.value)
                    .sort((a, b) => b.value - a.value);

                treemap(root);

                const node = d3.select(this.$el)
                    .selectAll(".node")
                    .data(root.leaves())
                    .enter()
                    .append("div")
                    .attr("class", "node")
                    .style("left", d => d.x0 + "px")
                    .style("top", d => d.y0 + 70 + "px")
                    .style("width", d => d.x1 - d.x0 + "px")
                    .style("height", d => d.y1 - d.y0 + "px");

                node.append("div")
                    .attr("class", "node-label")
                    .text(d => {
                        let text = '';
                        while (d.parent && d.parent.data.key) {
                            text = d.parent.data.key + '/' + text;
                            d = d.parent;
                        }
                        return text;
                    });

                node.append("div")
                    .attr("class", "node-value")
                    .text(d => format(d.value));
            });
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
</style>

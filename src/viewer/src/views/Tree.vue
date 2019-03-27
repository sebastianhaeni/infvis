<template>
    <div class="container" ref="container"></div>
</template>

<script lang="ts">
    import {Component, Vue} from 'vue-property-decorator';
    import {extractData} from "@/data/extract-data";
    import * as d3 from "d3";
    import assets from "../assets/demo-treemap.json";

    @Component
    export default class Tree extends Vue {

        mounted(): this {
            console.log(this.$refs);
            const width = this.$refs.container.clientWidth;
            const height = this.$refs.container.clientHeight;

            console.log(width, height);

            const samples = assets as Array<{ Files: Array<{ FilePath: string, LinesAdded: number }> }>;
            const data = extractData(samples);

            const treeChart = d3.tree().size([width, 800]);
            const root = d3.hierarchy(data);

            treeChart(root);

            // Nodes
            const svg = d3.select(this.$refs.container)
                .append('svg');

            const nodes = svg
                .append('g')
                .selectAll("circle.node")
                .data(root.descendants())
                .enter();

            nodes
                .append("circle")
                .classed("node", true)
                .attr("cx", (d) => d.x)
                .attr("cy", (d) => d.y)
                .attr("r", 4);
            nodes
                .append("text")
                .style("font", "normal 12px Arial")
                .attr("x", (d) => d.x)
                .attr("y", (d) => d.y + 15)
                .text((d) => d.data.name);

            // Links
            svg
                .append('g')
                .selectAll("line.link")
                .data(root.links())
                .enter()
                .append("line")
                .classed("link", true)
                .attr("x1", (d) => d.source.x)
                .attr("y1", (d) => d.source.y)
                .attr("x2", (d) => d.target.x)
                .attr("y2", (d) => d.target.y);

            return this;
        }
    }
</script>

<style lang="scss">
    .container {
        height: 100%;
    }

    svg {
        width: 100%;
    }
</style>

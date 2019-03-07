import * as d3 from "d3";
import {extractData} from "./extract-data";

import assets from "./assets/demo-treemap.json";

const samples = assets as Array<{ Files: Array<{ FilePath: string, LinesAdded: number }> }>;

const data = extractData(samples);

const treeLayout = d3.tree()
    .size([window.innerWidth, window.innerHeight]);

const root = d3.hierarchy(data);

treeLayout(root);

// Nodes
const nodes = d3.select("svg g.nodes")
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
d3.select("svg g.links")
    .selectAll("line.link")
    .data(root.links())
    .enter()
    .append("line")
    .classed("link", true)
    .attr("x1", (d) => d.source.x)
    .attr("y1", (d) => d.source.y)
    .attr("x2", (d) => d.target.x)
    .attr("y2", (d) => d.target.y);

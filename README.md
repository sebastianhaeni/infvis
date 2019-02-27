---
permalink: /index.html
---
# InfVis - Information Visualization
Information Visualization of a large Code Basis

#  Project Plan

## Deadlines

* Abstract submission by March 13, 2019
* Project submission by May 15, 2019

## Goal

Create 
1. a poster of size A3 with two chart display
2. an interactive web based chart

## Idea

Analyze a large code basis with associated commits, lines of code and folder structure.

### First chart

Treemap with Heatmap and Time series

http://mbostock.github.io/d3/talk/20111018/treemap.html

The goal is to visualize the source code how it grows over time (using commit data).
The files are highlighted in a heatmap fashion by amount of lines changes within the time slot chosen.
The treemap display top level folders (to a defined sub folder level), the size is respective to the amount of lines of code.
The folders are grouped by their parent folder and colorized.

This chart will
1. be interactable on web page by allowing the viewer to scroll through time using a slider or an automated playlist
2. be displayed on a poster with several small multiples

### Second Chart 

Radian dendrogram

https://observablehq.com/@d3/d3-radial-dendrogram

Displays the folder structure / tree structure in an interesting way that allows the viewer to kinda dive into folders.

## Technologies

We will use D3.js to visualize the data collected.
For preprocessing we will use several scripts that we will write in an arbitrary language chosen by us in the future.

## Optional

It would be awesome if the visualization works for an arbitrary Git code base.

# InfVis - Information Visualization [![Build Status](https://dev.azure.com/marcsallin/InfVis/_apis/build/status/InfVis-CI?branchName=master)](https://dev.azure.com/marcsallin/InfVis/_build/latest?definitionId=3&branchName=master)

Information Visualization of a large code base

## Abstract

The world becomes more and more driven by software. Thera are a lot of huge software intensive systems which have to be maintained and extended. A software intensive system is a special product. The single material it is made of, is source code. Therefore it exists only virtually. Often dozens of developers work on a system and it is difficult to keep track of it. Which components are regularly adapted? What should quality assurance focus on? In which areas should technical debt be reduced?

This project aims to visualize development activities, applied to the source code of a software intensive system, over time. To achieve this goal, the systems version control system (VCS) history will be mined to extract file manipulations. As a large code base contains thousands to millions of files, they are aggregated on the file system level (directory structure). To enable further drilldown, into an interesting aggregate, a treemap will be provided.

The data used to create the graphics are taken from a commercial software system written in C# and its git based VCS. The software system consists of around 50k lines of code, 4.5k files and 9k commits. A git based VCS records changes to a set of files over time. Each increment of changes to one or more files is called a commit. The following information will be extracted from every commit in the VCS history:

* Commit Date
* Path of file¹
* LinesAdded¹
* LinesDeleted¹

¹ per file in this commit

Tools to be used:

* VCS mining: C#, .NET Framework, libgit2sharp
* Visualization: JavaScript, D3.js

## Project Plan

### Deadlines

* Abstract submission by March 13, 2019
* Project submission by May 15, 2019

### Project Collaborators

* Sebastian Häni
* Marc Sallin

### Results

1. a poster of size A3 with two chart displays
2. an interactive web based chart

### Activities

**TODO**


## Technical Concept

This chapther covers the technical details of the solution.

### VCS Mining

**TODO**

### Data preperation

**TODO**

### Activity Map

Treemap with Heatmap and Time series

<http://mbostock.github.io/d3/talk/20111018/treemap.html>

The goal is to visualize the source code how it grows over time (using commit data).
The files are highlighted in a heatmap fashion by amount of lines changes within the time frame chosen.
The treemap displays folders down to a defined level, the size within the chart is respective to the amount of lines of code.
The folders are grouped by their parent folder and colorized.

This chart will

1. be interactable on web page by allowing the viewer to scroll through time using a slider or an automated playlist
2. be displayed on a poster with small multiples

### Aggregate Drilldown

Radian dendrogram

<https://observablehq.com/@d3/d3-radial-dendrogram>

Displays the folder structure / tree structure in an interesting way that allows the viewer to dive into folders visually.

# Notes

* <https://codescene.io/projects/168/jobs/11346/results/code/hotspots/system-map>
* <http://mbostock.github.io/d3/talk/20111116/pack-hierarchy.html>
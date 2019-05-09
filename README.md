# InfVis - Information Visualization [![Build Status](https://dev.azure.com/marcsallin/InfVis/_apis/build/status/InfVis-CI?branchName=master)](https://dev.azure.com/marcsallin/InfVis/_build/latest?definitionId=3&branchName=master)

Information Visualization of a large code base

## Abstract

Software maintenance/evolution activities are supported by providing a visual development activity hotspot analysis for software components. The activities are gathered from a version control system. The pipeline is pluggable and consists of the steps “Extraction”, “Aggregation”, “Visualization”. The resulting visualization is an interactive website, showing the overall activity as well as the evolution of the system components over time.

## Introduction

The world is driven by software. Out there are a lot of large software intensive systems which must be maintained and extended. Often dozens of developers work on such a system and it is difficult to keep track of change. A software intensive system is a special product. The single material it is made of is source code and therefore it exists only virtually. Different aspects such as the structure can only be visualized using specialized tools. To not slow down the development process and to prevent bugs, source code complexity must constantly be reduced. But which investment in complexity reduction pays off the most?

The aim of this project is to answer the above question by providing a visualization of development activities, to identify hotspots. For this purpose, the version control system (VCS) of a software intensive system is mined. File manipulations over time are extracted, aggregated and visualized.

## Methods and Material

The provided tool mines a git-based VCS. Git records changes to a set of files over time. Each increment of changes to one or more files is called a commit. The following information will be extracted from every commit in the VCS:

* Commit Date
* Path of file¹
* LinesAdded¹
* LinesDeleted¹

¹ per file in this commit

Tools to be used:

* VCS mining: C#, .NET Framework, libgit2sharp
* Visualization: TypeScript, vue.js, chart.js, D3.js

## Results

The final aggregation groups the commits into buckets, where every bucket represents a time span of 30 days. The activity (LOC added, deleted & modified) within each time span is aggregated on file level, using the file path as identifier.

The website offers two views. The first view shows the overall activity and growth of the source code base over time. The second view lets one browse through the history of the source code. Shows a view of a single time span. The webpage supports drilldown, to investigate a module. Hovering a sub component of a module shows their name and LOC count.

## Project Plan

### Deadlines

* Abstract submission by March 13, 2019
* Project submission by May 15, 2019

### Project Collaborators

* Sebastian Häni
* Marc Sallin

### Artifacts

1. a poster of size A3 with two chart displays
2. an interactive website for visualization

declare const d3: any;
declare const $: any;

export function treemap(o, data, domain) {
    const defaults = {
        margin: { top: 24, right: 0, bottom: 0, left: 0 },
        rootname: 'TOP',
        format: ',d',
        title: '',
        width: $('#chart').width(),
        height: $('#chart').height(),
    };

    let root;
    const opts = { ...defaults, ...o };
    const formatNumber = d3.format(opts.format);
    const rname = opts.rootname;
    const margin = opts.margin;
    const theight = 36 + 16;
    let currentDepth = 0;

    $('#chart').width(opts.width).height(opts.height);
    const width = opts.width - margin.left - margin.right;
    const height = opts.height - margin.top - margin.bottom - theight;
    let transitioning;

    const color = d3.scale.category20c()
        .domain(domain);

    const x = d3.scale.linear()
        .domain([0, width])
        .range([0, width]);

    const y = d3.scale.linear()
        .domain([0, height])
        .range([0, height]);

    const treemap = d3.layout.treemap()
        .children((d, depth) => depth ? null : d._children)
        .sort((a, b) => a.value - b.value)
        .ratio(height / width * 0.5 * (1 + Math.sqrt(5)))
        .mode('squarify') // slice, dice, slice-dice, squarify (ratio controlled)
        .round(false);

    const svg = d3.select('#chart').append('svg')
        .attr('width', width + margin.left + margin.right)
        .attr('height', height + margin.bottom + margin.top)
        .style('margin-left', -margin.left + 'px')
        .style('margin.right', -margin.right + 'px')
        .append('g')
        .attr('transform', 'translate(' + margin.left + ',' + margin.top + ')')
        .style('shape-rendering', 'crispEdges');

    const grandparent = svg.append('g')
        .attr('class', 'grandparent');

    grandparent.append('rect')
        .attr('y', -margin.top)
        .attr('width', width)
        .attr('height', margin.top);

    grandparent.append('text')
        .attr('x', 6)
        .attr('y', 6 - margin.top)
        .attr('dy', '.75em');

    if (data instanceof Array) {
        root = { key: rname, values: data };
    } else {
        root = data;
    }

    initialize(root);
    accumulate(root);
    layout(root);
    display(root);

    function initialize(root) {
        root.x = root.y = 0;
        root.dx = width;
        root.dy = height;
        root.depth = 0;
    }

    // Aggregate the values for internal nodes. This is normally done by the
    // treemap layout, but not here because of our custom implementation.
    // We also take a snapshot of the original children (_children) to avoid
    // the children being overwritten when when layout is computed.
    function accumulate(d) {
        return (d._children = d.values)
            ? d.value = d.values.reduce((p, v) => p + accumulate(v), 0)
            : d.value;
    }

    // Compute the treemap layout recursively such that each group of siblings
    // uses the same size (1×1) rather than the dimensions of the parent cell.
    // This optimizes the layout for the current zoom state. Note that a wrapper
    // object is created for the parent node for each group of siblings so that
    // the parent’s dimensions are not discarded as we recurse. Since each group
    // of sibling was laid out in 1×1, we must rescale to fit using absolute
    // coordinates. This lets us use a viewport to zoom.
    function layout(d) {
        if (d._children) {
            treemap.nodes({ _children: d._children });
            d._children.forEach(c => {
                c.x = d.x + c.x * d.dx;
                c.y = d.y + c.y * d.dy;
                c.dx *= d.dx;
                c.dy *= d.dy;
                c.parent = d;
                layout(c);
            });
        }
    }

    function display(d) {
        grandparent
            .datum(d.parent)
            .on('click', transition)
            .select('text')
            .text(name(d));

        const g1 = svg.insert('g', '.grandparent')
            .datum(d)
            .attr('class', 'depth');

        const g = g1.selectAll('g')
            .data(d._children)
            .enter()
            .append('g');

        g.filter(d => d._children)
            .classed('children', true)
            .on('click', transition);

        const children = g.selectAll('.child')
            .data(d => d._children || [d])
            .enter()
            .append('g');

        function getTitle(d) {
            return `${getName(d)} (${formatNumber(d.value)} lines total, ${formatNumber(getDelta(d))} lines changed)`;
        }

        function getName(d) {
            return d.key === 'undefined' ? d.parent.key : d.key;
        }

        children.append('rect')
            .attr('class', 'child')
            .call(rect)
            .append('title')
            .text(d => getTitle(d));

        g.append('rect')
            .attr('class', 'parent')
            .call(rect);

        const t = g.append('text')
            .attr('class', 'ptext')
            .attr('dy', '.75em');

        t.append('tspan')
            .text(d => getName(d));
        t.call(text);

        const changeDots = g.selectAll('.changed')
            .data(d => {
                const dots = [
                    [1, 20],
                    [21, 200],
                    [201, 2000],
                    [2001, Infinity],
                ];
                const changes = getDelta(d);
                const index = dots.findIndex((dot) => changes >= dot[0] && changes <= dot[1]);
                const changeDots = index >= 0 ? index + 1 : 0;
                return Array(changeDots).fill(null).map((n, i) => ({ d, i }));
            })
            .enter()
            .append('g');

        changeDots
            .append('circle')
            .attr('class', 'changed')
            .attr('fill', 'rgb(200, 0, 0)')
            .attr('r', 2)
            .call(changed);

        g.selectAll('rect')
            .style('fill', d => {
                // recursive function to group top level clusters with color
                function parent(d) {
                    return d.parent.key === 'Root' ? d : parent(d.parent);
                }

                return color(parent(d).key);
            });

        $('.child').tooltipster({
            delay: 0,
            side: 'bottom',
        });

        function transition(d?) {
            if (transitioning || !d) {
                return;
            }
            if (!d.values.every(val => val.key !== 'undefined')) {
                return;
            }

            transitioning = true;

            currentDepth = currentDepth === 0 ? 1 : currentDepth === 1 ? (d.depth === 0 ? 0 : 2) : 1;
            const g2 = display(d);
            const transitionDuration = 400;
            const t1 = g1.transition().duration(transitionDuration);
            const t2 = g2.transition().duration(transitionDuration);

            // Update the domain only after entering new elements.
            x.domain([d.x, d.x + d.dx]);
            y.domain([d.y, d.y + d.dy]);

            // Enable anti-aliasing during the transition.
            svg.style('shape-rendering', null);

            // Draw child nodes on top of parent nodes.
            svg.selectAll('.depth').sort((a, b) => a.depth - b.depth);

            // Fade-in entering text.
            g2.selectAll('text').style('fill-opacity', 0);

            // Transition to the new view.
            t1.selectAll('.ptext').call(text).style('fill-opacity', 0);
            t2.selectAll('.ptext').call(text).style('fill-opacity', 1);
            t1.selectAll('rect').call(rect);
            t2.selectAll('rect').call(rect);
            t1.selectAll('.changed').call(changed).style('fill-opacity', 0);
            t2.selectAll('.changed').call(changed).style('fill-opacity', 1);

            // Remove the old node when the transition is finished.
            t1.remove().each('end', () => {
                t1.selectAll('.ptext').call(text).style('fill-opacity', 0);
                t2.selectAll('.ptext').call(text).style('fill-opacity', 1);

                svg.style('shape-rendering', 'crispEdges');
                transitioning = false;
            });
        }

        return g;
    }

    function text(text) {
        text.selectAll('tspan')
            .attr('x', d => x(d.x) + 6);
        text.attr('x', d => x(d.x) + 6)
            .attr('y', d => y(d.y) + 6)
            .style('opacity', function (d) {
                this._originalTextContent = this._originalTextContent || this.textContent;
                this.textContent = this._originalTextContent;
                let textLength = this.getComputedTextLength();
                const padding = 6;
                const nodeWidth = this.previousSibling.getBBox().width;
                const ellipsis = '...';
                while (textLength > nodeWidth - 2 * padding && this.textContent.length > ellipsis.length) {
                    this.textContent = this.textContent.slice(2 + ellipsis.length);
                    this.textContent = ellipsis + this.textContent;
                    textLength = this.getComputedTextLength();
                }
                return 1;
            });
    }

    function changed(d) {
        d
            .attr('cx', d => x(d.d.x) + 9 + (d.i * 7))
            .attr('cy', d => y(d.d.y) + 30);
    }

    function getDelta(d) {
        function getDeltaRecursive(d) {
            return (d.delta || 0) + (d._children || []).reduce((sum, child) => sum + getDeltaRecursive(child), 0);
        }

        return getDeltaRecursive(d);
    }

    function rect(rect) {
        rect.attr('x', d => x(d.x))
            .attr('y', d => y(d.y))
            .attr('width', d => x(d.x + d.dx) - x(d.x))
            .attr('height', d => y(d.y + d.dy) - y(d.y))
            .attr('rx', '5')
            .attr('ry', '5');
    }

    function name(d, withDetails = true) {
        const fullName = d.parent
            ? `${name(d.parent, false)} / ${d.key}`
            : d.key;
        return withDetails
            ? `${fullName} (${formatNumber(d.value)} lines total, ${formatNumber(getDelta(d))} lines changed)`
            : fullName;
    }
}

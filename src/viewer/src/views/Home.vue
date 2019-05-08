<template>
    <section>
        <div class="wrapper">
            <canvas id="chart"></canvas>
        </div>
    </section>
</template>

<script lang="ts">
    import {Component, Vue} from 'vue-property-decorator';

    declare const Chart: any;
    declare const Color: any;

    @Component
    export default class Home extends Vue {

        inputs: any;

        async mounted() {

            const response = await fetch(process.env.VUE_APP_DATA_URL);
            const data = await response.json();
            let timestamp = data.map(d => new Date(d.StartTime));
            let elements = Object.values<any>(data).map(d => Object.values<any>(d.Elements));
            let totalLines = [];

            for (let i = 0; i < data.length; i++) {
                const all = data.slice(0, i)
                    .map(d => d.Elements)
                    .reduce((a, b, i) => {
                        // aggregate time slices and remove deleted files
                        const resetDelta = i + 1 < i;
                        Object.entries<any>(b).forEach(entry => {
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
                totalLines.push(Object.values<any>(all).reduce((acc, el) => acc + el.LinesOfCode, 0))
            }
            let changedLines = elements.map(slice => slice.reduce((acc, el) => acc + Math.abs(el.RelativeLinesDelta), 0));

            const presets = {
                red: 'rgb(255, 99, 132)',
                orange: 'rgb(255, 160, 66)',
                yellow: 'rgb(255, 205, 86)',
                green: 'rgb(75, 192, 192)',
                blue: 'rgb(54, 162, 235)',
                purple: 'rgb(153, 102, 255)',
                grey: 'rgb(201, 203, 207)'
            };
            this.inputs = {
                min: -100,
                max: 100,
                count: 8,
                decimals: 2,
                continuity: 1
            };

            const options = {
                maintainAspectRatio: false,
                spanGaps: false,
                elements: {
                    line: {
                        tension: 0.000001
                    }
                },
                plugins: {
                    filler: {
                        propagate: false
                    }
                },
                scales: {
                    xAxes: [{
                        ticks: {
                            autoSkip: false,
                            maxRotation: 0
                        }
                    }]
                }
            };
            const timeFormat = 'MM/DD/YYYY HH:mm';

            new Chart('chart', {
                type: 'line',
                data: {
                    labels: timestamp,
                    datasets: [{
                        backgroundColor: this.transparentize(presets.purple),
                        borderWidth: 0,
                        data: changedLines,
                        label: 'Changed lines',
                        fill: 'start',
                        pointRadius: 0
                    }, {
                        backgroundColor: this.transparentize(presets.blue),
                        borderWidth: 0,
                        data: totalLines,
                        label: 'Total lines',
                        fill: 'start',
                        pointRadius: 0
                    }]
                },
                options: Chart.helpers.merge(options, {
                    elements: {
                        line: {
                            tension: 0
                        }
                    },
                    scales: {
                        xAxes: [{
                            type: 'time',
                            time: {
                                parser: timeFormat,
                                tooltipFormat: 'll HH:mm'
                            },
                            gridLines: false,
                        }],
                        yAxes: [{
                            gridLines: false,
                            ticks: {
                                callback: value => value.toLocaleString()
                            }
                        }]
                    },
                    tooltips: {
                        mode: 'index', // tooltips are always activated on hover and point to all datapoints in the week
                        intersect: false, // mouse must not exactly be on point to trigger tooltip
                    },
                })
            });
        }

        transparentize(color, opacity?) {
            const alpha = opacity === undefined ? .8 : 1 - opacity;
            return Color(color).alpha(alpha).rgbString();
        }

        generateLabels(config?) {
            return this.months(Chart.helpers.merge({
                count: this.inputs.count,
                section: 3
            }, config || {}));
        }

        months(config) {
            const cfg = config || {};
            const count = cfg.count || 12;
            const section = cfg.section;
            const values = [];
            let i, value;

            for (i = 0; i < count; ++i) {
                value = MONTHS[Math.ceil(i) % 12];
                values.push(value.substring(0, section));
            }

            return values;
        }
    }

    const MONTHS = [
        'January',
        'February',
        'March',
        'April',
        'May',
        'June',
        'July',
        'August',
        'September',
        'October',
        'November',
        'December'
    ];
</script>

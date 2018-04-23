import Vue from 'vue';
import Component from 'vue-class-component';
import { Prop } from 'vue-property-decorator';
import * as Numeral from 'numeral';
//import { Bar } from 'vue-chartjs';
import { MetricsChart } from '../../metricschart';
import { MetricsChartData } from '../../metricschartdata';

@Component({
  components: {
    //    Bar: require('vue-chartjs')
    vuechart: require('../vuechart/vuechart.vue.html')
  }
})
export default class DashboardChartComponent extends Vue 
{
  @Prop() chart: MetricsChart;

  private barChartOptions: any =
    {
      scales: {
        xAxes: [{
          barPercentage: 0.4,
          gridLines: {
            display: false
          },
          ticks: {
            fontColor: "grey"
          }
        }],
        yAxes: [{
          ticks: {
            beginAtZero: true,
            fontColor: "grey",
            callback: (value: number, index: number, values: number[]) =>
            {
              return this.formatYAxisValue(this.chart, value);
            }
          },
          gridLines: {
            zeroLineColor: "grey"
          }
        }],
      },
      tooltips: {
        callbacks: {
          label: (tooltipItem: any, data: any) =>
          {
            let value = data.datasets[tooltipItem.datasetIndex].data[tooltipItem.index] || '';
            const label = Numeral(value).format("0,0");
            return (this.chart.type == "AMOUNT" ? "$" : "") + label;
          }
        }
      },
      animation: false,
      scaleShowVerticalLines: false,
      responsive: true,
      legend: { display: false }
    };

  get barChartData(): any
  {
    const data =
      {
        labels: this.getBarChartLabels(),
        datasets: [
          {
            data: this.getBarChartDataset(),
            backgroundColor: this.getBarChartColor()
          }
        ],
        options: this.barChartOptions
      };
    return data;
  }

  private getBarChartDataset(): any[]
  {
    let result = new Array();
    for (let i = 0; i < this.chart.chartData.length; i++)
    {
      result.push(this.chart.chartData[i].value);
    }

    return result;
  }

  private getBarChartLabels(): string[]
  {
    let result = new Array();
    for (let i = 0; i < this.chart.chartData.length; i++)
    {
      result.push(this.chart.chartData[i].periodName);
    }

    return result;
  }

  private getBarChartColor(): string
  {
    return this.chart.color;
  }

  private getBarChartOptions(): any
  {
    return this.barChartOptions;
  }

  private get hasData(): boolean
  {
    for (let i = 0; i < this.chart.chartData.length; i++)
    {
      if (this.chart.chartData[i].value > 0) return true;
    }
    return false;
  }

  private formatYAxisValue(chart: MetricsChart, value: number): string
  {
    const suffix = ["", "k", "M", "G", "T", "P", "E"];
    let index = 0;
    let dvalue = value;
    while ((value /= 1000) >= 1 && ++index) dvalue /= 1000;
    let result =
      (chart.type == "AMOUNT" ? "$" : "") +
      Math.round(dvalue).toString() + suffix[index];
    return result;
  }

  get formattedTotalValue()
  {
    const s = Numeral(this.chart.totalValue).format("0,0");
    return (this.chart.type == "AMOUNT" ? "$" : "") + s;
  }
}

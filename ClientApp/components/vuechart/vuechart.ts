import Vue from 'vue';
import Component from 'vue-class-component';
import { Prop, Watch } from 'vue-property-decorator';
import { Chart } from 'chart.js' // With moment.js


@Component
export default class VueChart extends Vue
{
  @Prop() width: number;
  @Prop() height: number;
  @Prop() type: string; // 'line', 'bar', 'radar', 'polarArea', 'pie', 'doughnut', 'bubble', 'scatter'

  private m_data: any = {};

  private chart: Chart;

  get data(): any
  {
    return this.m_data;
  }

  @Prop() set data(value: any)
  {
    this.m_data = value;
  }

  mounted()
  {
    this.resetChart()
  }

  updated()
  {
    this.resetChart()
  }
  
  resetChart()
  {
    if (this.chart) this.chart.destroy();

    this.chart = new Chart(this.$el as any, {
      type: this.type,
      data: this.data,
      options: this.data.options,
      plugins: this.data.plugins
    });
  }
}
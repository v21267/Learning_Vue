import Vue from 'vue';
import Component from 'vue-class-component';
import VenvitoService from '../../venvitoservice';
import { MetricsChart } from '../../metricschart';
import { MetricsChartData } from '../../metricschartdata';

@Component({
  components: {
    dashboardchart: require('../dashboardchart/dashboardchart.vue.html')
  }
})
export default class DashboardComponent extends Vue 
{
  dateRange: string = "7";
  charts: MetricsChart[] = new Array<MetricsChart>();

  mounted()
  {
    this.loadCharts();
  }

  loadCharts()
  {
    VenvitoService.getMetricsChart(this.dateRange)
      .then(data => { this.charts = data; })
      .catch(error => console.log(error.response));
  }

  setDateRange(dateRange: string, e: any)
  {
    this.dateRange = dateRange;
    this.loadCharts();
  }

  get hasCharts(): boolean
  {
    return (this.charts != null && this.charts.length > 0);
  }
}

import Vue from 'vue';
import Component from 'vue-class-component';
import VenvitoService from '../../venvitoservice';
import { MetricsData } from '../../metricsdata';

@Component({
  components: {
    dateswitcher: require('../dateswitcher/dateswitcher.vue.html'),
    activityrow: require('../activityrow/activityrow.vue.html')
  }
})
export default class ActivitiesComponent extends Vue
{
  get metricsData(): MetricsData[]
  {
    return this.$store.state.metricsData;
  }

  get hasData(): boolean
  {
    return (this.metricsData.length > 0);
  }
}

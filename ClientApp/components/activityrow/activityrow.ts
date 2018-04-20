import Vue from 'vue';
import Component from 'vue-class-component';
import { Prop } from 'vue-property-decorator';
import VenvitoService from '../../venvitoservice';
import { MetricsData } from '../../metricsdata';

@Component
export default class ActivityRowComponent extends Vue
{
  private inAmountEditing: boolean = false;
  private originalAmount: number;
  @Prop() data: MetricsData;
}

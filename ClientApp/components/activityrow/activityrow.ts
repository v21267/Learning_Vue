import Vue from 'vue';
import Component from 'vue-class-component';
import { Prop } from 'vue-property-decorator';
import * as Numeral from 'numeral';
import VenvitoService from '../../venvitoservice';
import { MetricsData } from '../../metricsdata';

@Component
export default class ActivityRowComponent extends Vue
{
  private inAmountEditing: boolean = false;
  private originalAmount: number;

  @Prop() data: MetricsData;

  updateMetricsData(md: MetricsData)
  {
    VenvitoService.updateMetricsData(md)
      .catch(error => console.log(error.response));
  }

  updateCount(delta: number)
  {
    if (this.data.value + delta < 0) return;

    this.data.value += delta;
    this.updateMetricsData(this.data);
  }

  get isZeroCount(): boolean
  {
    return (this.data.value == 0);
  }

  get formattedAmount(): string
  {
    return Numeral(this.data.value).format("0,0");
  }


  updated()
  {
    if (this.inAmountEditing)
    {
      (this.$refs.amountInput as any).focus();
    }
  }


  editAmount()
  {
    this.originalAmount = this.data.value;
    this.inAmountEditing = true;
  }

  setAmount()
  {
    if (this.isInvalidAmount) return;

    this.updateMetricsData(this.data);
    this.inAmountEditing = false;
  }

  cancelAmountEditing()
  {
    this.data.value = this.originalAmount;
    this.inAmountEditing = false;
  }

  get isInvalidAmount(): boolean
  {
    return !(this.data.value >= 0 && this.data.value <= 999999999 &&
             this.data.value != null && (this.data.value as any) != "");
  }

  get saveAmountButonColor(): string
  {
    return (this.isInvalidAmount ? "silver" : "green");
  }
}

import Vue from 'vue';
import Component from 'vue-class-component';
import VenvitoService from '../../venvitoservice';


@Component({
})
export default class DateSwitcherComponent extends Vue 
{
  private m_calendarMenu: boolean;

  constructor()
  {
    super();
    this.m_calendarMenu = false;
  }

  get calendarMenu(): boolean
  {
    return this.m_calendarMenu;
  }

  set calendarMenu(value: boolean)
  {
    this.m_calendarMenu = value;
  }

  formatForDatePicker(date: Date): string
  {
    if (date.toString() == "Invalid Date") return "";

    const s = VenvitoService.dateToString(date);
    return s.substr(0, 4) + '-' + s.substr(4, 2) + '-' + s.substr(6, 2);
  }

  get calendarDate(): string
  {
    return this.formatForDatePicker(this.currentDate);
  }

  set calendarDate(value: string)
  {
    const year = parseInt(value.substr(0, 4));
    const month = parseInt(value.substr(5, 2));
    const day = parseInt(value.substr(8, 2));
    const newDate = new Date(year, month - 1, day);
    this.setCurrentDate(newDate);
  }

  get currentDate(): Date
  {
    return this.$store.state.currentDate;
  }

  get currentDateCaption(): String
  {
    return (this.isToday ? "Today" :
      this.isYesterday ? "Yesterday" :
        this.currentDate.toDateString());
  }

  setCurrentDate(newDate: Date)
  {
    this.$store.dispatch("setCurrentDate", newDate);

    const data = VenvitoService.getMetricsData(newDate)
      .then(data => this.$store.dispatch("setMetricsData", data))
      .catch(error => console.log(error.response));
  }

  shiftDate(delta: number)
  {
    const newDate = VenvitoService.addDays(this.currentDate, delta);
    this.setCurrentDate(newDate);
  }

  get isToday(): Boolean
  {
    return (this.currentDate.toDateString() ==
      new Date().toDateString());
  }

  get isYesterday(): Boolean
  {
    return (this.currentDate.toDateString() ==
      VenvitoService.addDays(new Date(), -1).toDateString());
  }

  get today(): Date
  {
    return new Date();
  }
}

import axios, { AxiosPromise } from 'axios';
import { MetricsData } from "./metricsdata";
import { MetricsChart } from './metricschart';

export default class VenvitoService
{
  static initialDate: Date = new Date();

  static addDays(date: Date, delta: number): Date
  {
    return new Date(date.getTime() + delta * (1000 * 60 * 60 * 24));
  }

  static dateToString(date: Date): string
  {
    const d: string =
      (date.getFullYear().toString() as any).padStart(4, "0") +
      ((date.getMonth() + 1).toString() as any).padStart(2, "0") +
      (date.getDate().toString() as any).padStart(2, "0");
    return d;
  }

  static async getMetricsData(date: Date): Promise<MetricsData[]>
  {
    const d: string = VenvitoService.dateToString(date);
    const url: string = '/api/MetricsData/' + d;
    const response = await axios.get(url);
    const result = response.data as Promise<MetricsData[]>;
    return result;
  }

  static async updateMetricsData(data: MetricsData): Promise<any>
  {
    const url: string = '/api/MetricsData/';
    const response = await axios.post(url, data);
    return response;
  }

  static async getMetricsChart(dateRange: string): Promise<MetricsChart[]>
  {
    const url: string = '/api/MetricsChart/' + dateRange;
    const response = await axios.get(url);
    const result = response.data as Promise<MetricsChart[]>;
    return result;
  }
}

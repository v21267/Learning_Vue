import { MetricsDefinition } from './metricsdefinition';
import { MetricsChartData } from './metricschartdata';

export class MetricsChart extends MetricsDefinition
{
  totalValue: number;
  chartData: MetricsChartData[];
}

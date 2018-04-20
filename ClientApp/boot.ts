import './css/site.css';
import './stylus/main.styl';

//import 'bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import 'vuetify/dist/vuetify.min.css';
import Vue from 'vue';
import VueRouter from 'vue-router';
import Vuex from 'vuex';
import Vuetify from 'vuetify';
import VenvitoService from './venvitoservice';
import { MetricsData } from './metricsdata';

Vue.use(VueRouter);
Vue.use(Vuex);
Vue.use(Vuetify);

const routes = [
    { path: '/activities', component: require('./components/activities/activities.vue.html') },
    { path: '/dashboard', component: require('./components/dashboard/dashboard.vue.html') },
    { path: '/', redirect: '/activities' }
];

const store = new Vuex.Store({
  state: {
    currentDate: VenvitoService.initialDate,
    metricsData: new Array() as MetricsData[]
  },
  mutations: {
    setCurrentDate(state, newDate: Date)
    {
      state.currentDate = newDate;
    },
    setMetricsData(state, data: MetricsData[])
    {
      state.metricsData = data;
    },
    updateMetricsData(state, md: MetricsData)
    {
      const updatedData = state.metricsData.map(item =>
      {
        if (item.code === md.code)
        {
          item.value = md.value;
        }
      })
    }
  },
  actions: {
    setCurrentDate(context, newDate: Date)
    {
      context.commit('setCurrentDate', newDate)
    },
    setMetricsData(context, data: MetricsData[])
    {
      context.commit('setMetricsData', data)
    },
    updateMetricsData(context, md: MetricsData)
    {
      context.commit('updateMetricsData', md)
    }
  }
})

VenvitoService.getMetricsData(VenvitoService.initialDate).then(
  (data) =>
  {
    store.dispatch('setMetricsData', data);
  });


new Vue({
  el: '#app-root',
  store,
  router: new VueRouter({ mode: 'history', routes: routes }),
  render: h => h(require('./components/app/app.vue.html'))
});

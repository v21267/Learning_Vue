import Vue from 'vue';
import Component from 'vue-class-component';

@Component({
  components: {
    apptitle: require('../apptitle/apptitle.vue.html')
  }
})
export default class AppComponent extends Vue
{
}

import { createApp } from 'vue'
import './style.css'
import Antd from 'ant-design-vue';
import App from './App.vue';

const app = createApp(App);

app.use(Antd).mount('#app');

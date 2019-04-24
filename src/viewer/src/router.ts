import Vue from 'vue';
import Router from 'vue-router';
import Home from './views/Home.vue';
import Treemap from './views/Treemap.vue';
import Dendrogram from './views/Dendrogram.vue';

Vue.use(Router);

export default new Router({
    routes: [
        {
            path: '/',
            name: 'home',
            component: Home,
        },
        {
            path: '/treemap',
            name: 'treemap',
            component: Treemap,
        },
        {
            path: '/dendrogram',
            name: 'dendrogram',
            component: Dendrogram,
        },
    ],
});

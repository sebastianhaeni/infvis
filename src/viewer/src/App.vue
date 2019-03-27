<template>
    <div id="app">
        <div id="nav">
            <router-link to="/">Home</router-link>
            |
            <router-link to="/tree">Tree</router-link>
            |
            <router-link to="/treemap">Treemap</router-link>
            |
            <router-link to="/dendrogram">Dendrogram</router-link>
        </div>
        <router-view/>
    </div>
</template>

<style lang="scss">
    body {
        padding: 0;
        margin: 0;
        min-height: 100vh;
        min-width: 100vh;
    }

    #app {
        min-height: calc(100% - 30px);
        min-width: 100%;
        font-family: 'Avenir', Helvetica, Arial, sans-serif;
        -webkit-font-smoothing: antialiased;
        -moz-osx-font-smoothing: grayscale;
        text-align: center;
        color: #2c3e50;
    }

    #nav {
        padding: 30px 30px 10px;

        a {
            font-weight: bold;
            color: #2c3e50;

            &.router-link-exact-active {
                color: #42b983;
            }
        }
    }
</style>

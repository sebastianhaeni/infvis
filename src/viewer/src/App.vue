<template>
    <div id="app">
        <div id="nav">
            <router-link to="/">Line Chart</router-link>
            |
            <router-link to="/treemap">Tree Map</router-link>
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
        min-height: 100%;
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

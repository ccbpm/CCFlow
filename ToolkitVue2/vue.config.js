module.exports = {
    publicPath: '/', //publicPath取代了baseUrl
    outputDir: 'dist',
    assetsDir: 'static',
    lintOnSave: true,
    runtimeCompiler: true, //关键点在这  原来的 Compiler 换成了 runtimeCompiler
    // 调整内部的 webpack 配置。
    // 查阅 https://github.com/vuejs/vue-doc-zh-cn/vue-cli/webpack.md
    chainWebpack: () => { },
    configureWebpack: () => { },
    // 配置 webpack-dev-server 行为。
    devServer: {
        open: process.platform === 'darwin',
        proxy: { //可以代理多个项目
            //第一个代理，这里的/api1和/api2就对应了第一步的 baseURL
            // /api/xxxx
            // /ccflow/api/start/xxx
            "/api": {
                target: "http://101.43.52.116:8085/",//https://uatworkflow.mapfarm.com/", //只要是以/api开头的链接都会被代理到 这个target属性所代表的位置（我这里是：http://help.jflow.cn:8081/）
                //http://101.43.52.116:8085/
                //target: "http://localhost:2296/",//https://uatworkflow.mapfarm.com/", //只要是以/api开头的链接都会被代理到 这个target属性所代表的位置（我这里是：http://help.jflow.cn:8081/）
                ws: false,
                changeOrigin: true,
                cookieDomainRewrite: {
                    "*": ""
                },
                cookiePathRewrite: {
                    "*": ""
                },
                timeout: 300000, //设置超时时间
                pathRewrite: {
                    "^/api": "" //这里是将/api替换为空字符串“” ，也就是删除的意思
                }
            }
        }
    }
}

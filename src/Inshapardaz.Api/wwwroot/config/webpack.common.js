var webpack = require('webpack');
var HtmlWebpackPlugin = require('html-webpack-plugin');
var ExtractTextPlugin = require('extract-text-webpack-plugin');
var CopyWebpackPlugin = require('copy-webpack-plugin');

const path = require('path');
const rootDir = path.resolve(__dirname, '..');

module.exports = {
    entry: {
        'polyfills': './src/polyfills.ts',
        'vendor': './src/vendor.ts',
        'app': './src/main.ts'
    },

    resolve: {
        extensions: ['.js', '.ts'],
        alias: {
            jquery: "jquery/src/jquery"
        }
    },

    module: {
        loaders: [
            {
                test: /\.ts$/,
                loaders: ['awesome-typescript-loader', 'angular2-template-loader']
            }, {
                test: /\.html$/,
                loader: 'html-loader',
                options: {
                    minimize: true,
                    removeComments: true,
                    collapseWhitespace: true,
              
                    // angular 2 templates break if these are omitted
                    removeAttributeQuotes: false,
                    keepClosingSlash: true,
                    caseSensitive: true,
                    conservativeCollapse: true,
                  }              
            }, {
                test: /\.(png|jpe?g|gif|svg|woff|woff2|ttf|eot|ico)$/,
                loader: 'file-loader?name=assets/[name].[hash].[ext]'
            }, {
                test: /\.scss$/,
                exclude: /node_modules/,
                loaders: ['raw-loader', 'sass-loader']
            }, {
                test: /\.css$/,
                use: [ 'style-loader', 'css-loader' ]
            }
        ]
    },

    plugins: [
        new webpack.optimize.CommonsChunkPlugin({
            name: ['app', 'vendor', 'polyfills']
        }),
        new HtmlWebpackPlugin({
            template: './src/index.html'
        }),
        new CopyWebpackPlugin([{
            from: './src/assets/img',
            to: './images'
        }, {
            from: './src/i18n',
            to: './assets/i18n'
        }]),
        new webpack.ProvidePlugin({
            $: 'jquery',
            jQuery: 'jquery',
            'window.$': 'jquery',
            'window.jQuery': 'jquery',
        })
    ]
};

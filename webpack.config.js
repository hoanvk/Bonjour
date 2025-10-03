const path = require('path');

module.exports = {
    entry: './resources/js/main.js',
    output: {
        filename: 'main.js',
        path: path.resolve(__dirname, 'wwwroot', 'dist'),
    },
    devServer: {
        static: path.resolve(__dirname, 'wwwroot', 'dist'),
        port: 8080,
        hot: true,
    },
    module: {
        rules: [
            {
                test: /\.s?css$/,
                use: ['style-loader', 'css-loader', 'sass-loader'],
            },
            {
                test: /\.(png|svg|jpg|jpeg|gif|webp)$/i,
                type: 'asset',
                loader: 'file-loader',
                options: {
                    name: 'images/[name].[ext]',
                },
            },
            {
                test: /\.(eot|woff(2)?|ttf|otf|svg)$/i,
                type: 'asset',
            },
        ],
    },
};

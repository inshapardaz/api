@Echo ====== Restore node packages ======
@call npm install
@Echo ====== Building SPA ======
@call node_modules/.bin/webpack -p --config ./config/webpack.prod.js -p
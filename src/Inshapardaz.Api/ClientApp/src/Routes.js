import React from 'react';
import { Route } from 'react-router';
import { Switch, BrowserRouter as Router } from 'react-router-dom';

import history from './state/index.js';

import MainLayout from './components/layout/mainLayout';
import RouteWithLayout from './components/layout/routeWithLayout';

import AuthorizeRoute from './components/api-authorization/AuthorizeRoute';
import ApiAuthorizationRoutes from './components/api-authorization/ApiAuthorizationRoutes';
import { ApplicationPaths } from './components/api-authorization/ApiAuthorizationConstants';

import Home from './components/home.jsx';

import AdminPage from './components/admin/adminpage.jsx';
import BooksPage from './components/books/booksPage.jsx';
import AuthorsPage from './components/authors/authorsPage.jsx';
import SeriesPage from './components/series/seriesPage.jsx';
import CategoriesPage from './components/categories/categoriesPage.jsx';

class Routes extends React.Component
{
	render ()
	{
		return (
			<Router history={history}>
				<Switch>
					<RouteWithLayout layout={MainLayout} path="/" component={Home} exact/>
					<RouteWithLayout layout={MainLayout} path="/authors" component={AuthorsPage} />
					<RouteWithLayout layout={MainLayout} path="/books" component={BooksPage} />
					<RouteWithLayout layout={MainLayout} exact path="/series" component={SeriesPage} />
					<RouteWithLayout layout={MainLayout} exact path="/categories" component={CategoriesPage} />
					<AuthorizeRoute layout={MainLayout} path="/admin" component={AdminPage} />
					<Route path={ApplicationPaths.ApiAuthorizationPrefix} component={ApiAuthorizationRoutes} />
				</Switch>
			</Router>
		);
	}
}

export default Routes;

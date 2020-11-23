import React from 'react';
import Divider from '@material-ui/core/Divider';
import { makeStyles } from '@material-ui/core/styles';
import CssBaseline from '@material-ui/core/CssBaseline';
import Header from '../header/header.jsx';
import Footer from '../footer/footer.jsx';

const useStyles = () => makeStyles((theme) => ({
	root : {
		padding : theme.spaces(48)
	}
}));
const classes = useStyles();

const MainLayout = (props) =>
{
	console.log("render layout");
	const { children } = props;
	return (
		<>
			<CssBaseline />
			<Header />
			<main className={classes.root}>
				{children}
			</main>
			<Divider />
			<Footer />
		</>
	);
};

export default MainLayout;

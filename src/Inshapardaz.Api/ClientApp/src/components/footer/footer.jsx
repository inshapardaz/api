import React, { Component } from 'react';
import { FormattedMessage } from 'react-intl';
import Typography from '@material-ui/core/Typography';
import Link from '@material-ui/core/Link';
import { makeStyles } from '@material-ui/core/styles';

export default class Footer  extends Component
{
	useStyles = () => makeStyles((theme) => ({
		footer : {
		  backgroundColor : theme.palette.background.paper,
		  padding : theme.spacing(6)
		}
	}));

	copyright ()
	{
		return (
		  <Typography variant="body2" color="textSecondary" align="center">
				{'Copyright © '}
				<Link color="inherit" href="/">
					<FormattedMessage id="app" />
				</Link>{' '}
				{new Date().getFullYear()}
				{'.'}
		  </Typography>
		);
	}

	render ()
	{
		const classes = this.useStyles();
		return (
			<footer className={classes.footer}>
				<Typography variant="h6" align="center" gutterBottom>
					<FormattedMessage id="app" />
				</Typography>
				<Typography variant="subtitle1" align="center" color="textSecondary" component="p">
				</Typography>
				{this.copyright()}
		  </footer>
		);
	}
}

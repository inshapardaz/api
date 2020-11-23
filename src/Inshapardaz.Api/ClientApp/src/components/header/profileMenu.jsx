/* eslint-disable no-script-url */
/* eslint-disable no-mixed-spaces-and-tabs */
import React, { useEffect, useState, useRef } from 'react';
import { FormattedMessage, injectIntl } from 'react-intl';
import { Link } from 'react-router-dom';
import Button from '@material-ui/core/Button';
import AccountCircle from '@material-ui/icons/AccountCircle';
import Grow from '@material-ui/core/Grow';
import Paper from '@material-ui/core/Paper';
import Popper from '@material-ui/core/Popper';
import MenuItem from '@material-ui/core/MenuItem';
import ClickAwayListener from '@material-ui/core/ClickAwayListener';
import MenuList from '@material-ui/core/MenuList';
import Avatar from '@material-ui/core/Avatar';
import KeyboardArrowDownIcon from '@material-ui/icons/KeyboardArrowDown';
import AuthService from '../../services/AuthorizeService';
import { ApplicationPaths } from '../api-authorization/ApiAuthorizationConstants';

const ProfileMenu = () =>
{
	const anchorRef = useRef(null);
	const [open, setOpen] = useState(false);
	const [profile, setProfile] = useState(null);
	const [isAuthenticated, setAuthenticated] = useState(false);

	const populateState = async () =>
	{
		const [isAuth, user] = await Promise.all([AuthService.isAuthenticated(), AuthService.getUser()]);
		setAuthenticated(isAuth);
		console.dir(user);
		setProfile(user);
	};

	useEffect(() =>
	{
		let subscription = AuthService.subscribe(() => populateState());
		populateState();

		return () =>
		{
			AuthService.unsubscribe(subscription);
		};
	}, []);

	const handleToggle = () =>
	{
		setOpen((prevOpen) => !prevOpen);
	};

	const handleClose = (event) =>
	{
		if (anchorRef.current && anchorRef.current.contains(event.target))
		{
		  return;
		}

		setOpen(false);
	};

	const displayName = isAuthenticated && profile ? profile.name : '';

	let renderMenu = null;
	if (isAuthenticated)
	{
		const avatar = profile && profile.picture ? profile.picture : '';
		const profilePath = `${ApplicationPaths.Profile}`;
		const logoutPath = { pathname : `${ApplicationPaths.LogOut}`, state : { local : true } };
		renderMenu = (
			<>
				<Button
					edge="end"
					aria-label="account of current user"
					aria-controls="login"
					aria-haspopup="true"
					onClick={handleToggle}
					ref={anchorRef}
					color="inherit"
					startIcon={<Avatar src={avatar}/>}
					endIcon={<KeyboardArrowDownIcon />}
				>
				</Button>
				<Popper open={open} anchorEl={anchorRef.current} transition disablePortal>
					{({ TransitionProps, placement }) => (
						<Grow
							{...TransitionProps}
							style={{ transformOrigin : placement === 'bottom' ? 'center top' : 'center bottom' }}
						>
							<Paper>
								<ClickAwayListener onClickAway={handleClose}>
									<MenuList autoFocusItem={open} id="menu-list-grow" onKeyDown={handleClose}>
										<MenuItem component={Link} onClick={ handleClose} to={profilePath}>{displayName}</MenuItem>
										<MenuItem component={Link} onClick={handleClose} to={logoutPath}><FormattedMessage id="logout" /></MenuItem>
									</MenuList>
								</ClickAwayListener>
							</Paper>
						</Grow>
					)}
				</Popper>
			</>
		);
	}
	else
	{
		const registerPath = `${ApplicationPaths.Register}`;
		const loginPath = `${ApplicationPaths.Login}`;

		console.dir(registerPath);
		renderMenu = (
			<>
				<Button
					edge="end"
					aria-label="account of current user"
					aria-controls="login"
					aria-haspopup="true"
					onClick={handleToggle}
					ref={anchorRef}
					color="inherit"
					startIcon={<AccountCircle />}
					endIcon={<KeyboardArrowDownIcon />}
				>
				</Button>
				<Popper open={open} anchorEl={anchorRef.current} transition disablePortal>
					{({ TransitionProps, placement }) => (
						<Grow
							{...TransitionProps}
							style={{ transformOrigin : placement === 'bottom' ? 'center top' : 'center bottom' }}
						>
							<Paper>
								<ClickAwayListener onClickAway={handleClose}>
									<MenuList autoFocusItem={open} id="menu-list-grow" onKeyDown={handleClose}>
										<MenuItem component={Link} onClick={handleClose} to={registerPath}><FormattedMessage id="register" /></MenuItem>
										<MenuItem component={Link} onClick={handleClose} to={loginPath}><FormattedMessage id="login" /></MenuItem>
									</MenuList>
								</ClickAwayListener>
							</Paper>
						</Grow>
					)}
				</Popper>
			</>
		);
	}

	return renderMenu;
};

export default injectIntl(ProfileMenu);

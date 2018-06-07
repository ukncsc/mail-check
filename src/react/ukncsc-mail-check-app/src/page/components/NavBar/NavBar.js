import React from 'react';
import { Link } from 'react-router-dom';
import { Container, Menu } from 'semantic-ui-react';
import includes from 'lodash/includes';
import startsWith from 'lodash/startsWith';

import './NavBar.css';

const VersatileLink = ({ link, external, children }) =>
  // waiting on https://github.com/airbnb/javascript/pull/1648 to fix below
  // eslint-disable-next-line jsx-a11y/anchor-is-valid
  external ? <a href={link}>{children}</a> : <Link to={link}>{children}</Link>;

const LinkedMenuItem = ({ name, link, pathname, external }) => (
  <VersatileLink link={link} external={external}>
    <Menu.Item name={name} active={startsWith(pathname, link)} />
  </VersatileLink>
);

export default ({ location, routes, userRole }) => (
  <Menu className="NavBar" attached="top" size="huge" stackable>
    <Container>
      {routes.left
        .filter(
          ({ roleTypes }) => (roleTypes ? includes(roleTypes, userRole) : true)
        )
        .map(route => (
          <LinkedMenuItem
            pathname={location.pathname}
            key={route.link}
            {...route}
          />
        ))}
      <Menu.Menu position="right">
        {routes.right
          .filter(
            ({ roleTypes }) =>
              roleTypes ? includes(roleTypes, userRole) : true
          )
          .map(route => (
            <LinkedMenuItem
              pathname={location.pathname}
              key={route.link}
              {...route}
            />
          ))}
      </Menu.Menu>
    </Container>
  </Menu>
);

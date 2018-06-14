import React from 'react';
import { Container, Grid, Header, Image } from 'semantic-ui-react';
import { Link } from 'react-router-dom';
import CurrentUserMenu from '../CurrentUserMenu';
import './Banner.css';

export default ({ companyLogo, productName, currentUser }) => (
  <Container className="Banner">
    <Grid columns={2} stackable>
      <Grid.Row>
        <Grid.Column>
          {
            // waiting on https://github.com/airbnb/javascript/pull/1648 to fix below
            /* eslint-disable jsx-a11y/anchor-is-valid */
          }
          <Link to="/">
            <Image src={companyLogo} size="medium" />
          </Link>
        </Grid.Column>
        <Grid.Column floated="right" textAlign="right">
          <Header as="h4">
            <CurrentUserMenu {...currentUser} />
          </Header>
          <Header as="h3">{productName}</Header>
        </Grid.Column>
      </Grid.Row>
    </Grid>
  </Container>
);

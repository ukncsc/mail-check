import React from 'react';
import { Link } from 'react-router-dom';
import { Container, Grid, Image, List, Segment } from 'semantic-ui-react';
import { companyLogoInverted } from 'page/assets';
import { footerLinks } from 'page/data';

import './PageFooter.css';

const LinksColumn = ({ link }) => (
  <Grid.Column width={link.width}>
    <List inverted>
      <List.Header as="h4">{link.header}</List.Header>
      {link.items.map(item => (
        <List.Item href={item.href} key={item.href}>
          {item.icon && <List.Icon name={item.icon} />}
          {item.name}
        </List.Item>
      ))}
    </List>
  </Grid.Column>
);

export default () => (
  <Container className="PageFooter" fluid>
    <Segment attached="bottom" inverted>
      <Container>
        <Grid inverted stackable divided stretched>
          <Grid.Column width={6} verticalAlign="middle">
            {
              // waiting on https://github.com/airbnb/javascript/pull/1648 to fix below
              /* eslint-disable jsx-a11y/anchor-is-valid */
            }
            <Link to="/">
              <Image src={companyLogoInverted} height="80" />
            </Link>
          </Grid.Column>
          {footerLinks.map(link => (
            <LinksColumn link={link} key={link.header} />
          ))}
        </Grid>
      </Container>
    </Segment>
  </Container>
);

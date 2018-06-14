import React from 'react';
import { Button, Container, Grid, Message } from 'semantic-ui-react';
import { Link } from 'react-router-dom';

export default ({ show, onAgree }) =>
  show && (
    <Container style={{ paddingTop: 20 }}>
      <Message floating warning>
        <Message.Header>Terms of Service</Message.Header>
        <Message.Content>
          <Grid columns={2} stackable>
            <Grid.Column width={9} verticalAlign="middle">
              By using Mail Check you are agreeing to our use of cookies and our{' '}
              {
                // waiting on https://github.com/airbnb/javascript/pull/1648 to fix below
                /* eslint-disable jsx-a11y/anchor-is-valid */
              }
              <Link to="/terms">Terms and Conditions of Service</Link>.
            </Grid.Column>
            <Grid.Column width={3} textAlign="right" verticalAlign="middle">
              <Button onClick={onAgree} color="green" fluid>
                Agree
              </Button>
            </Grid.Column>
          </Grid>
        </Message.Content>
      </Message>
    </Container>
  );

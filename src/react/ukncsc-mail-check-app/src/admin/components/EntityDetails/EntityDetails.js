import React from 'react';
import { Grid, Header, Card, Icon } from 'semantic-ui-react';

export default ({ title, id, name, email }) => (
  <Grid>
    <Grid.Row columns={1}>
      <Grid.Column>
        <Header as="h2">{title}</Header>
      </Grid.Column>
    </Grid.Row>
    <Grid.Row columns={1}>
      <Grid.Column>
        <Card fluid>
          <Card.Content>
            <Card.Header>{name}</Card.Header>
          </Card.Content>
          <Card.Content extra>
            <React.Fragment>
              <Icon name="id card outline" />
              {id}
            </React.Fragment>
            {email && (
              <React.Fragment>
                <span style={{ paddingRight: 10 }} />
                <Icon name="mail outline" />
                {email}
              </React.Fragment>
            )}
          </Card.Content>
        </Card>
      </Grid.Column>
    </Grid.Row>
  </Grid>
);

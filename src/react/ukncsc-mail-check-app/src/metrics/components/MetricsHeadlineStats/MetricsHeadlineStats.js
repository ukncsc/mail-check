import React from 'react';
import { Grid, Statistic } from 'semantic-ui-react';

const colourScale = ['green', 'yellow', 'blue'];
export default ({ values, size = 'huge' }) => (
  <Grid columns={Object.keys(values).length} textAlign="center" stackable>
    <Grid.Row>
      {Object.keys(values).map((key, i) => (
        <Grid.Column key={key}>
          <Statistic
            color={colourScale[i]}
            size={size}
            value={values[key]}
            label={key}
          />
        </Grid.Column>
      ))}
    </Grid.Row>
  </Grid>
);

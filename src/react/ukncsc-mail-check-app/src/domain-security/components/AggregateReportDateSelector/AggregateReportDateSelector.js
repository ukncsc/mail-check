import React from 'react';
import { Form, Grid } from 'semantic-ui-react';
import periodOptions from './periodOptions.json';

export default ({
  className,
  id,
  fetchDomainSecurityAggregateData,
  selectedDays,
  includeSubdomains = true,
}) => (
  <React.Fragment>
    <Form>
      <Grid stackable style={{ paddingBottom: 20 }}>
        <Grid.Row>
          <Grid.Column width="9">
            <Form.Dropdown
              value={selectedDays}
              options={periodOptions}
              className={className}
              onChange={(e, data) =>
                fetchDomainSecurityAggregateData(
                  id,
                  data.value,
                  includeSubdomains
                )
              }
            />
          </Grid.Column>
          <Grid.Column width="3">
            <Form.Checkbox
              toggle
              checked={includeSubdomains}
              label="Include subdomains"
              onChange={() =>
                fetchDomainSecurityAggregateData(
                  id,
                  selectedDays,
                  !includeSubdomains
                )
              }
            />
          </Grid.Column>
        </Grid.Row>
      </Grid>
    </Form>
  </React.Fragment>
);

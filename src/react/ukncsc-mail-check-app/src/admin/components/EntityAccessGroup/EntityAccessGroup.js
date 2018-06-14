import React from 'react';
import { Button, Header, Input, Label, Loader, Grid } from 'semantic-ui-react';

const EntityAccessGroupList = ({ items, onRemove }) =>
  items.length > 0 ? (
    <Label.Group style={{ paddingTop: 10 }}>
      {items.map(item => (
        <Label
          size="big"
          content={item.name || `${item.firstName} ${item.lastName}`}
          onRemove={onRemove && (() => onRemove(item))}
          key={item.id}
        />
      ))}
    </Label.Group>
  ) : (
    <p>Nothing to show</p>
  );

export default ({ title, results = [], isLoading, onRemove, onAdd }) => (
  <Grid stackable>
    <Grid.Row>
      <Grid.Column width={onAdd ? 7 : 9}>
        <Header as="h2">{title}</Header>
      </Grid.Column>
      {onAdd && (
        <Grid.Column width={2}>
          <Button fluid primary onClick={onAdd} content="Add" />
        </Grid.Column>
      )}
      <Grid.Column width={2} textAlign="right">
        <Input fluid icon="filter" input={{ placeholder: 'Filter...' }} />
      </Grid.Column>
    </Grid.Row>
    <Grid.Row columns={1}>
      <Grid.Column>
        {isLoading && <Loader inline active />}
        {!isLoading && (
          <EntityAccessGroupList items={results} onRemove={onRemove} />
        )}
      </Grid.Column>
    </Grid.Row>
  </Grid>
);

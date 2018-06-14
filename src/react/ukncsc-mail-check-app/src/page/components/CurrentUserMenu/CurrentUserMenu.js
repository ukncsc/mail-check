import React from 'react';
import { Icon, Loader, Dropdown } from 'semantic-ui-react';

export default ({ isLoading, error, user }) => {
  if (isLoading) {
    return <Loader size="small" active inline />;
  }

  if (error) {
    return (
      <React.Fragment>
        <Icon name="exclamation circle" />
        {error}
      </React.Fragment>
    );
  }

  if (user) {
    return (
      <Dropdown
        pointing="top right"
        trigger={user.firstName}
        options={[
          {
            key: 'logout',
            icon: 'sign out',
            text: 'Log out',
            onClick: () => {
              window.location.href = '/callback?logout=';
            },
          },
        ]}
        header={
          <Dropdown.Header
            as="h5"
            icon="user circle"
            content={`${user.firstName} ${user.lastName}`}
          />
        }
      />
    );
  }

  return null;
};

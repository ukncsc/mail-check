import React from 'react';
import { Divider } from 'semantic-ui-react';
import { MailCheckMessage } from 'common/components';

export default ({
  addDomainOnClick,
  error,
  hasSearched,
  lastSearchTerm,
  loading,
  searchTermIsPublicSectorOrg,
}) => {
  if (loading || !hasSearched) {
    return null;
  }

  if (error) {
    return (
      <React.Fragment>
        <MailCheckMessage error>{error.message}</MailCheckMessage>
        <Divider hidden />
      </React.Fragment>
    );
  }

  if (searchTermIsPublicSectorOrg) {
    return (
      <React.Fragment>
        <MailCheckMessage info>
          {lastSearchTerm} is not currently monitored by Mail Check.{' '}
          <a href="" onClick={addDomainOnClick}>
            Add {lastSearchTerm} to Mail Check
          </a>.
        </MailCheckMessage>
        <Divider hidden />
      </React.Fragment>
    );
  }

  return (
    <React.Fragment>
      <MailCheckMessage info>
        {lastSearchTerm} is not currently monitored by Mail Check. Please email{' '}
        <a
          href={`mailto:mailcheck@digital.ncsc.gov.uk?Subject=Add ${lastSearchTerm} to Mail Check`}
        >
          mailcheck@digital.ncsc.gov.uk
        </a>{' '}
        to have {lastSearchTerm} added to Mail Check.
      </MailCheckMessage>
      <Divider hidden />
    </React.Fragment>
  );
};

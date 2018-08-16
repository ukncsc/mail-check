import React from 'react';
import PropTypes from 'prop-types';
import { Divider, Message } from 'semantic-ui-react';
import moment from 'moment';
import { BackLink } from 'common/components';
import {
  DomainSecurityTitle,
  DomainSecurityDetailsMessages,
} from 'domain-security/components';

const DomainSecurityDetailsMx = ({
  mxId,
  hostname,
  type,
  loading,
  error,
  records,
  children,
}) => {
  const record = mxId
    ? records.find(r => String(r.id) === mxId)
    : records.find(r => String(r.hostname) === hostname);

  return (
    <React.Fragment>
      <BackLink />
      <DomainSecurityTitle
        title={type}
        subtitle={record && record.hostname}
        loading={loading}
        failures={record && record.failures}
        warnings={record && record.warnings}
        inconclusives={record && record.inconclusives}
      >
        {record &&
          record.lastChecked && (
            <p>Last checked {moment(record.lastChecked).fromNow()}</p>
          )}
      </DomainSecurityTitle>
      <Divider hidden />
      {children}
      {error && <Message error>{error.message}</Message>}
      {!loading &&
        !error &&
        record && (
          <DomainSecurityDetailsMessages
            type={type}
            failures={record.failures}
            warnings={record.warnings}
            inconclusives={record.inconclusives}
          />
        )}
    </React.Fragment>
  );
};

DomainSecurityDetailsMx.defaultProps = {
  mxId: null,
  hostname: null,
  loading: false,
  error: null,
  records: [],
};

DomainSecurityDetailsMx.propTypes = {
  mxId: PropTypes.string,
  hostname: PropTypes.string,
  type: PropTypes.string.isRequired,
  loading: PropTypes.bool,
  error: PropTypes.shape({ message: PropTypes.string }),
  records: PropTypes.arrayOf(
    PropTypes.shape({
      hostname: PropTypes.string.isRequired,
      lastChecked: PropTypes.string.isRequired,
      failures: PropTypes.arrayOf(PropTypes.string),
      warnings: PropTypes.arrayOf(PropTypes.string),
      inconclusives: PropTypes.arrayOf(PropTypes.string),
    })
  ),
};

export default DomainSecurityDetailsMx;

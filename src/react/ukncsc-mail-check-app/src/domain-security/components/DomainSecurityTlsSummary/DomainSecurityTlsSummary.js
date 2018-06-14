import React from 'react';
import PropTypes from 'prop-types';
import { Link } from 'react-router-dom';
import map from 'lodash/map';
import { Header, Message } from 'semantic-ui-react';
import {
  DomainSecurityMessage,
  DomainSecuritySummaryMessages,
} from 'domain-security/components';
import { DomainSecurityContext } from 'domain-security/context';

const DomainSecurityTlsSummary = ({
  id,
  loading,
  error,
  records,
  pending,
  children,
}) => (
  <React.Fragment>
    {children}
    <p>
      <Link to="/domain-security/tls-advice">View NCSC advice on TLS</Link>
    </p>
    {!loading &&
      !error &&
      records && (
        <React.Fragment>
          {map(records, record => (
            <React.Fragment key={record.id}>
              <Header as="h3">{record.hostname}</Header>
              <p>
                <DomainSecurityContext.Consumer>
                  {value => (
                    <Link to={`/${value}/${id}/tls/${record.id}`}>
                      View more information
                    </Link>
                  )}
                </DomainSecurityContext.Consumer>
              </p>
              <DomainSecuritySummaryMessages
                id={record.id}
                type="TLS"
                failures={record.failures.length}
                warnings={record.warnings.length}
                inconclusives={record.inconclusives.length}
              />
            </React.Fragment>
          ))}
          {!pending &&
            records &&
            !records.length && (
              <DomainSecurityMessage success>
                No MX records found.
              </DomainSecurityMessage>
            )}
          {pending && (
            <DomainSecurityMessage info={pending}>
              No MX records found yet.
            </DomainSecurityMessage>
          )}
        </React.Fragment>
      )}
    {error && (
      <Message error>
        There was a problem retrieving the TLS summary for this domain:{' '}
        {error.message}
      </Message>
    )}
  </React.Fragment>
);

DomainSecurityTlsSummary.defaultProps = {
  id: null,
  loading: false,
  error: null,
  records: null,
  pending: false,
};

DomainSecurityTlsSummary.propTypes = {
  id: PropTypes.string,
  loading: PropTypes.bool,
  error: PropTypes.shape({ message: PropTypes.string }),
  records: PropTypes.arrayOf(
    PropTypes.shape({
      id: PropTypes.string,
      hostname: PropTypes.string,
      failures: PropTypes.arrayOf(PropTypes.string),
      warnings: PropTypes.arrayOf(PropTypes.string),
      inconclusives: PropTypes.arrayOf(PropTypes.string),
    })
  ),
  pending: PropTypes.bool,
};

export default DomainSecurityTlsSummary;

import React from 'react';
import PropTypes from 'prop-types';
import { Link } from 'react-router-dom';
import map from 'lodash/map';
import toLower from 'lodash/toLower';
import { Message } from 'semantic-ui-react';
import {
  DomainSecurityRecord,
  DomainSecuritySummaryMessages,
} from 'domain-security/components';
import { isPending } from 'common/helpers';
import { DomainSecurityContext } from 'domain-security/context';

const DomainSecurityRecordSummary = ({ item, type, children }) => (
  <React.Fragment>
    {children}
    {map(item.records, ({ record }, i) => (
      <DomainSecurityRecord key={i} inheritedFrom={item.inheritedFrom}>
        {record}
      </DomainSecurityRecord>
    ))}
    {!item.loading &&
      !item.error &&
      item.records && (
        <React.Fragment>
          <p>
            <DomainSecurityContext.Consumer>
              {value => (
                <Link to={`/${value}/${item.id}/${toLower(type)}`}>
                  View more information
                </Link>
              )}
            </DomainSecurityContext.Consumer>
          </p>
          <DomainSecuritySummaryMessages
            id={item.id}
            type={type}
            failures={item.failures.length}
            warnings={item.warnings.length}
            inconclusives={item.inconclusives.length}
          />
        </React.Fragment>
      )}
    {isPending(item) && (
      <Message info>
        {`We don't have any information about the ${type} record of this domain
          yet. Please check back later or get in touch if you think there's a
          problem.`}
      </Message>
    )}
    {item.error && (
      <Message error>
        There was a problem retrieving the {type} summary for this domain:{' '}
        {item.error.message}
      </Message>
    )}
  </React.Fragment>
);

DomainSecurityRecordSummary.propTypes = {
  type: PropTypes.oneOf(['DMARC', 'SPF', 'TLS']).isRequired,
  item: PropTypes.shape({
    id: PropTypes.string,
    loading: PropTypes.bool,
    error: PropTypes.shape({ message: PropTypes.string }),
    records: PropTypes.arrayOf(PropTypes.shape({ record: PropTypes.string })),
    inheritedFrom: PropTypes.shape({
      id: PropTypes.number,
      name: PropTypes.string,
    }),
    failures: PropTypes.arrayOf(PropTypes.string),
    warnings: PropTypes.arrayOf(PropTypes.string),
    inconclusives: PropTypes.arrayOf(PropTypes.string),
  }).isRequired,
};

export default DomainSecurityRecordSummary;

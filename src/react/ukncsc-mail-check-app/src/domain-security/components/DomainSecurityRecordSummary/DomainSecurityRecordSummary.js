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
import { DomainSecurityContext } from 'domain-security/context';

const DomainSecurityRecordSummary = ({
  type,
  description,
  id,
  loading,
  error,
  records,
  inheritedFrom,
  failures,
  warnings,
  inconclusives,
}) => (
  <React.Fragment>
    <p>{description}</p>
    {map(records, ({ record }, i) => (
      <DomainSecurityRecord key={i} inheritedFrom={inheritedFrom}>
        {record}
      </DomainSecurityRecord>
    ))}
    {!loading &&
      !error &&
      (records ? (
        <React.Fragment>
          <p>
            <DomainSecurityContext.Consumer>
              {value => (
                <Link to={`/${value}/${id}/${toLower(type)}`}>
                  View more information
                </Link>
              )}
            </DomainSecurityContext.Consumer>
          </p>
          <DomainSecuritySummaryMessages
            id={id}
            type={type}
            failures={failures.length}
            warnings={warnings.length}
            inconclusives={inconclusives.length}
          />
        </React.Fragment>
      ) : (
        <Message info>
          {`We don't have any information about the ${type} record of this domain
          yet. Please check back later or get in touch if you think there's a
          problem.`}
        </Message>
      ))}
    {error && (
      <Message error>
        There was a problem retrieving the {type} summary for this domain:{' '}
        {error.message}
      </Message>
    )}
  </React.Fragment>
);

DomainSecurityRecordSummary.defaultProps = {
  id: null,
  loading: false,
  error: null,
  records: null,
  inheritedFrom: null,
  failures: [],
  warnings: [],
  inconclusives: [],
};

DomainSecurityRecordSummary.propTypes = {
  type: PropTypes.oneOf(['DMARC', 'SPF', 'TLS']).isRequired,
  description: PropTypes.string.isRequired,
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
};

export default DomainSecurityRecordSummary;

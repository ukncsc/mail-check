import React from 'react';
import { Link } from 'react-router-dom';
import filter from 'lodash/filter';
import map from 'lodash/map';
import toUpper from 'lodash/toUpper';
import { Divider, Message } from 'semantic-ui-react';
import {
  DomainSecurityDetailsMessages,
  DomainSecurityRecord,
  DomainSecurityRecordExplanation,
  DomainSecurityTitle,
} from 'domain-security/components';
import { BackLink, ShowMoreDropdown } from 'common/components';
import { DomainSecurityContext } from 'domain-security/context';

const DomainSecurityDetails = ({
  match,
  domain,
  canViewAggregateData,
  ...props
}) => {
  const type = toUpper(match.params.type);
  const item = props[match.params.type] || {};
  const tagsProperty = match.params.type === 'spf' ? 'terms' : 'tags';

  return (
    <React.Fragment>
      <DomainSecurityContext.Consumer>
        {value => <BackLink link={`/${value}/${match.params.domainId}`} />}
      </DomainSecurityContext.Consumer>
      <DomainSecurityTitle
        loading={item.loading}
        error={item.error}
        subtitle={domain.name}
        failures={item.failures}
        warnings={item.warnings}
        inconclusives={item.inconclusives}
        lastChecked={item.lastChecked}
      >
        {type}
      </DomainSecurityTitle>
      {canViewAggregateData && (
        <DomainSecurityContext.Consumer>
          {value => (
            <p>
              <Link
                to={`/${value}/${match.params.domainId}/dmarc/aggregate-export`}
              >
                Export aggregate report data
              </Link>
            </p>
          )}
        </DomainSecurityContext.Consumer>
      )}
      {item.error && <Message error>{item.error.message}</Message>}
      {!item.error &&
        !item.loading && (
          <React.Fragment>
            {map(item.records, r => (
              <React.Fragment key={r.index}>
                <DomainSecurityRecord inheritedFrom={item.inheritedFrom}>
                  {r.record}
                </DomainSecurityRecord>
                <ShowMoreDropdown title="Explain record">
                  <DomainSecurityRecordExplanation
                    title="Discovered Tags"
                    data={filter(r[tagsProperty], t => !t.isImplicit)}
                  />
                  <DomainSecurityRecordExplanation
                    title="Implicit Tags"
                    data={filter(r[tagsProperty], t => t.isImplicit)}
                  />
                </ShowMoreDropdown>
                <Divider hidden />
              </React.Fragment>
            ))}
            <DomainSecurityDetailsMessages
              type={type}
              failures={item.failures}
              warnings={item.warnings}
              inconclusives={item.inconclusives}
            />
          </React.Fragment>
        )}
    </React.Fragment>
  );
};

export default DomainSecurityDetails;

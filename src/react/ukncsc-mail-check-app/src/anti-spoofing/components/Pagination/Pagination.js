import React from 'react';
import { Menu } from 'semantic-ui-react';
import range from 'lodash/range';

export default ({ page, pageSize, collectionSize, selectPage }) => {
  const createFirstPreviousPages = () => [
    { page: 1, display: '<<', active: false, disabled: page === 1 },
    { page: page - 1, display: '<', active: false, disabled: page - 1 <= 0 },
  ];

  const createStartPages = () => [
    { page: 1, display: '1', active: false, disabled: page === 1 },
    { page: -1, display: '...', active: false, disabled: true },
  ];

  const createMiddlePages = (start, pageCount, maxSize) =>
    range(start, start + Math.min(pageCount, maxSize)).map(_ => ({
      page: _,
      display: _.toString(),
      active: _ === page,
      disabled: false,
    }));

  const createEndPages = (maxSize, pageCount) =>
    pageCount > maxSize
      ? [
          { page: -1, display: '...', active: false, disabled: true },
          {
            page: pageCount,
            display: pageCount.toString(),
            active: false,
            disabled: page === pageCount,
          },
        ]
      : [];

  const createNextLastPages = pageCount => [
    {
      page: page + 1,
      display: '>',
      active: false,
      disabled: pageCount < page + 1,
    },
    {
      page: pageCount,
      display: '>>',
      active: false,
      disabled: page === pageCount,
    },
  ];

  const createPages = () => {
    const maxSize = 5;
    const pageCount = Math.ceil(collectionSize / pageSize);
    if (page < maxSize) {
      return createFirstPreviousPages()
        .concat(createMiddlePages(1, pageCount, maxSize))
        .concat(createEndPages(maxSize, pageCount))
        .concat(createNextLastPages(pageCount));
    } else if (page > pageCount + 1 - maxSize) {
      return createFirstPreviousPages()
        .concat(createStartPages())
        .concat(createMiddlePages(pageCount + 1 - maxSize, pageCount, maxSize))
        .concat(createNextLastPages(pageCount));
    } else if (page >= maxSize) {
      const offset = Math.floor(maxSize / 2);
      return createFirstPreviousPages()
        .concat(createStartPages())
        .concat(createMiddlePages(page - offset, pageCount, maxSize))
        .concat(createEndPages(maxSize, pageCount))
        .concat(createNextLastPages(pageCount));
    }
    return [];
  };

  const pages = createPages();

  return (
    <Menu floated="right" pagination>
      {pages.map((v, i) => (
        <Menu.Item
          key={i}
          active={v.active}
          disabled={v.disabled}
          onClick={v.disabled || v.active ? () => {} : () => selectPage(v.page)}
        >
          {v.display}
        </Menu.Item>
      ))}
    </Menu>
  );
};

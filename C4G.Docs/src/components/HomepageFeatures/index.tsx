import type {ReactNode} from 'react';
import clsx from 'clsx';
import Heading from '@theme/Heading';
import styles from './styles.module.css';

type FeatureItem = {
  title: string;
  emoji: string;
  description: ReactNode;
};

const FeatureList: FeatureItem[] = [
  {
    title: 'Zero Configuration',
    emoji: '⚡',
    description: (
      <>
        C4G works out of the box with minimal setup. Just configure your Google Sheets
        integration and start managing your game configs immediately.
      </>
    ),
  },
  {
    title: 'Auto Code Generation',
    emoji: '🔧',
    description: (
      <>
        Automatically generates C# DTO classes from your Google Sheets structure.
        No manual coding required - focus on your game logic instead.
      </>
    ),
  },
  {
    title: 'Production Ready',
    emoji: '🚀',
    description: (
      <>
        Built for reliability and performance. Thoroughly tested with high code
        coverage and used in production environments.
      </>
    ),
  },
  {
    title: 'Type Safe',
    emoji: '🔒',
    description: (
      <>
        Strong typing with custom type parsers for primitives, enums, and custom
        aliases. Catch errors at compile time, not runtime.
      </>
    ),
  },
  {
    title: 'Google Sheets Integration',
    emoji: '📊',
    description: (
      <>
        Leverage the familiar Google Sheets interface for configuration management.
        Enable non-programmers to manage game configs with ease.
      </>
    ),
  },
  {
    title: 'Modular Architecture',
    emoji: '🧩',
    description: (
      <>
        Clean, modular design with facade pattern. Easily extend with custom type
        parsers and integrate into your existing workflow.
      </>
    ),
  },
];

function Feature({title, emoji, description}: FeatureItem) {
  return (
    <div className={clsx('col col--4')}>
      <div className="text--center padding-horiz--md">
        <div className={styles.featureEmoji}>{emoji}</div>
        <Heading as="h3">{title}</Heading>
        <p>{description}</p>
      </div>
    </div>
  );
}

export default function HomepageFeatures(): ReactNode {
  return (
    <section className={styles.features}>
      <div className="container">
        <div className="row">
          {FeatureList.map((props, idx) => (
            <Feature key={idx} {...props} />
          ))}
        </div>
      </div>
    </section>
  );
}

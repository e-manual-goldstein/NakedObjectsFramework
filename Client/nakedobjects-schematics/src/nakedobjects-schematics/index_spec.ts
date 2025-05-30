import { Tree } from '@angular-devkit/schematics';
import { SchematicTestRunner } from '@angular-devkit/schematics/testing';
import * as path from 'path';


const collectionPath = path.join(__dirname, '../collection.json');


describe('nakedobjects-schematics', () => {
  it('works', () => {
    const runner = new SchematicTestRunner('schematics', collectionPath);
    runner.
        runSchematic('nakedobjects-schematics', {}, Tree.empty()).
        then(t => expect(t.files).toEqual([]) );
  });
});

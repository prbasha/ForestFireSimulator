# ForestFireSimulator
A forest fire simulator. Based on the Drossel and Schwabl definition of the forest-fire model.

At any time, each cell can be in one of three states:
* Empty
* Tree
* Burning

During an update, the following rules are applied to each cell:
* A burning cell becomes an empty cell
* A tree cell becomes a burning cell if one of its 8 neighbours is burning
* A tree cell becomes a burning cell as per the lightning probability
* An empty cell becomes a tree cell as per the regrowth probability

The lightning and regrowth probabilities can be updated in real-time.

A left-mouse click can be used to set a single tree on fire.

For more information: https://rosettacode.org/wiki/Forest_fire

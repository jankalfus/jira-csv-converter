# jira-csv-converter
This simple tool converts *a CSV export of stories from JIRA* into *a CSV that can be easily loaded into a neo4j database*.

The outputted CSV can be imported into neo4j like so:

```
LOAD CSV WITH HEADERS FROM 'file:///jira-edited.csv' AS row
MERGE (i:Issue {issueKey: row.Key, summary: row.Key + " " + row.Summary })
WITH i, row
FOREACH (blocksKey in CASE WHEN exists(row.Blocks) THEN split(row.Blocks, ":") ELSE [] END |
	MERGE (blocks:Issue {issueKey: blocksKey})
	MERGE (i)-[r:BLOCKS]->(blocks)
)
FOREACH (splitKey in CASE WHEN exists(row.Split) THEN split(row.Split, ":") ELSE [] END |
    MERGE (splitInto:Issue {issueKey: splitKey})
	MERGE (i)-[r:SPLIT_INTO]->(splitInto)
);
```

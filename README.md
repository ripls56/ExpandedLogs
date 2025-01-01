# ExpandedLogs

Enhances the default logging capabilities in Vintage Story, providing more detailed and useful logs for tracking various in-game events. 

The mod allows you to monitor the following actions:
- Block Breakage and Placement: The mod records all instances of block destruction and placement, enabling you to track changes in the game world.
- Inventory Interactions: Logs include information on all inventory-related actions, such as adding, removing, and moving items.
- Block Interactions: The mod tracks all player interactions with blocks, including activating mechanisms, using chests, and other containers.
- Entity Interactions: Logs contain information on all interactions with entities, including attacks, kills, and using entities for various purposes.
- Integration with Grafana: ExpandedLogs is specifically designed to work with Grafana, a popular platform for data visualization and monitoring. This allows users to easily analyze and visualize logs, creating informative dashboards and reports.

### Requirments
 - docker
 - docker compose
 - loki plugin

### Installation
 - [docker](https://docs.docker.com/engine/install/) and [docker compose](https://docs.docker.com/compose/install/linux/).
 - loki plugin `docker plugin install grafana/loki-docker-driver:2.9.4 --alias loki --grant-all-permissions`

### Run
Clone [repo](https://github.com/ripls56/vs-server-expandedlog-example) with compose and simple configs

U need to change grafana and loki timezones in docker-compose.yaml

Run it with `docker compose up -d`. If u need any server changes such as some kind of environments u can check [vintage story docker image](https://github.com/jsknnr/vintage-story-server) readme.
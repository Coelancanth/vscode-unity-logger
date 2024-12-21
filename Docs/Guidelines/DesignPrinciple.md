# SOLID 原则及相关设计原则

## 1. S - 单一职责原则（Single Responsibility Principle, SRP）
**定义**：一个类应该只有一个引起它变化的原因。也就是说，一个类只负责一件事。

- **目标**：降低类的复杂性，增强其可读性和可维护性。
- **违反的表现**：
  - 类中包含不相关的功能（例如，既负责数据处理又负责展示逻辑）。
  - 代码修改时，往往会影响多个功能。
- **示例**：
  - 错误：`UserManager` 类既负责用户数据管理，也负责发送邮件。
  - 改进：拆分为 `UserManager`（用户数据管理）和 `EmailService`（邮件发送）。
- **问题引导**：
  - 这个类的职责是否过多？
  - 修改一项功能时是否会意外影响其他功能？

---

## 2. O - 开闭原则（Open-Closed Principle, OCP）
**定义**：软件实体（类、模块、函数等）应该对扩展开放，对修改关闭。

- **目标**：通过增加新功能来扩展系统，而无需修改现有代码。
- **违反的表现**：
  - 为添加新功能，不得不修改已有代码。
  - 每次需求变化导致大量代码被重写。
- **实现方法**：
  - 使用抽象类或接口，让具体实现易于扩展。
  - 通过策略模式或工厂模式来实现。
- **示例**：
  - 错误：直接修改现有方法以支持新业务逻辑。
  - 改进：通过添加新类实现新逻辑，而不修改已有类。
- **问题引导**：
  - 添加新功能时，是否需要修改现有代码？
  - 是否可以通过扩展实现需求变化？

---

## 3. L - 里氏替换原则（Liskov Substitution Principle, LSP）
**定义**：子类必须能够替换其基类，并确保系统行为不变。

- **目标**：保持继承关系的正确性，确保代码的复用性和稳定性。
- **违反的表现**：
  - 子类无法正确实现父类的功能。
  - 使用子类对象时需要额外检查类型或功能。
- **核心理念**：
  - 子类不能违背父类的约定（如接口契约）。
  - 子类应该增强或维持基类功能，而不是削弱。
- **示例**：
  - 错误：一个矩形类有一个子类正方形，正方形改变宽度时会破坏矩形的逻辑。
  - 改进：通过抽象类或独立实现来区分矩形和正方形。
- **问题引导**：
  - 子类是否完全遵循父类的行为规范？
  - 在使用子类替换父类时，是否会出现逻辑错误？

---

## 4. I - 接口隔离原则（Interface Segregation Principle, ISP）
**定义**：客户端不应该被迫依赖它不需要的接口。也就是说，应该将大的接口拆分为更小、更具体的接口。

- **目标**：减少类对无关功能的依赖，增强系统的灵活性和清晰性。
- **违反的表现**：
  - 接口包含过多方法，导致实现类需要编写不必要的空实现。
  - 一个类因为依赖了过大的接口而变得复杂。
- **示例**：
  - 错误：`Shape` 接口中既有 `draw()` 方法，又有 `resize()` 方法，但某些实现只需要绘制功能。
  - 改进：拆分为 `Drawable` 和 `Resizable` 接口，分别满足不同需求。
- **问题引导**：
  - 接口中的方法是否被所有实现类使用？
  - 是否可以将接口拆分为更小的粒度？

---

## 5. D - 依赖倒置原则（Dependency Inversion Principle, DIP）
**定义**：高层模块不应该依赖低层模块；二者都应该依赖于抽象。抽象不应该依赖具体实现，具体实现应该依赖抽象。

- **目标**：减少模块之间的依赖，使系统更灵活、更易扩展。
- **违反的表现**：
  - 高层模块直接依赖具体实现，导致代码难以更改。
  - 更换底层实现时需要修改高层逻辑。
- **实现方法**：
  - 使用接口或抽象类来定义依赖关系。
  - 通过依赖注入（Dependency Injection）或工厂模式解耦。
- **示例**：
  - 错误：`OrderService` 类直接依赖 `MySQLDatabase`。
  - 改进：`OrderService` 依赖一个 `Database` 接口，而具体实现由 `MySQLDatabase` 或其他数据库类提供。
- **问题引导**：
  - 系统是否依赖了具体实现而非抽象？
  - 是否可以通过依赖注入替换具体的依赖关系？

---

# 其他设计原则

## Composition Over Inheritance（组合优于继承）
- **核心思想**：优先使用对象组合，而非深度继承层次。
- **应用场景**：设计灵活、解耦的系统时。

---

## High Cohesion, Low Coupling（高内聚低耦合）
- **核心思想**：模块内部功能应高度相关，而模块之间的依赖尽量少。
- **应用场景**：复杂系统中模块化设计。

---

## Separation of Concerns（关注点分离）
- **核心思想**：每个模块或层次应专注于单一职责，避免逻辑混杂。
- **应用场景**：在设计多层架构（如 MVC）时特别重要。

---

## Law of Demeter（迪米特法则）
- **核心思想**：模块之间尽量少交流，只与直接相关的模块交互。
- **应用场景**：减少系统中模块之间的耦合。
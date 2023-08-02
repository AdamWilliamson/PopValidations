//using System;
//using System.Collections.Generic;
//using PopValidations.Execution.Stores;
//using PopValidations.FieldDescriptors;
//using PopValidations.FieldDescriptors.Base;

//namespace PopValidations.Scopes.ForEachs;

//internal class ForEachIndexedScope<TValidationType, TFieldType> : ScopeBase
//{
//    public override bool IgnoreScope => false;
//    private readonly int index;

//    //private readonly FieldDescriptor<IEnumerable<TFieldType?>, TFieldType?> fieldDescriptor;
//    private readonly Action<IFieldDescriptor<IEnumerable<TFieldType?>, TFieldType?>> actions;

//    public override string Name => "Nothing";

//    public ForEachIndexedScope(
//        ValidationConstructionStore store,
//        int index,
//        //FieldDescriptor<IEnumerable<TFieldType?>, TFieldType?> fieldDescriptor,
//        Action<IFieldDescriptor<IEnumerable<TFieldType?>, TFieldType?>> actions
//    ) : base(store)
//    {
//        this.index = index;
//        //this.fieldDescriptor = fieldDescriptor;
//        this.actions = actions;
//    }

//    protected override void InvokeScopeContainer(ValidationConstructionStore store, object? value)
//    {
//        var thingo = new FieldDescriptor<IEnumerable<TFieldType?>, TFieldType?>(
//                    new IndexedPropertyExpressionToken<IEnumerable<TFieldType?>, TFieldType?>(
//                        $"[{index}]",
//                        index
//                    ),
//                    store
//                );

//        actions.Invoke(thingo);
//    }

//    protected override void InvokeScopeContainerToDescribe(ValidationConstructionStore store)
//    {
//        var thingo = new FieldDescriptor<IEnumerable<TFieldType?>, TFieldType?>(
//            new IndexedPropertyExpressionToken<IEnumerable<TFieldType?>, TFieldType?>(
//                $"[n]",
//                -1
//            ),
//            //this.validatorStore
//            store
//        );

//        //store.PushFieldDescriptor(thingo);

//        actions.Invoke(thingo);
//        //store.PopFieldDescriptor();
//    }
//}

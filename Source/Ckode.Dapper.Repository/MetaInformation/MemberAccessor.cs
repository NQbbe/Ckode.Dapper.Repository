﻿using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Ckode.Dapper.Repository.MetaInformation
{
	public class MemberAccessor
	{
		internal readonly string name;
		public string Name => name;
		private static readonly Func<object, object> NoGetter =
			obj => { throw new InvalidOperationException("No getter for property"); };
		private static readonly Action<object, object> NoSetter =
			(obj, val) => { throw new InvalidOperationException("No setter for property"); };

		public object GetValue(object target) { return getter(target); }
		public void SetValue(object target, object value) { setter(target, value); }
		internal readonly Func<object, object> getter;
		internal readonly Action<object, object> setter;

		public bool HasGetter => getter != NoGetter;
		public bool HasSetter => setter != NoSetter;

		public MemberAccessor(PropertyInfo property)
		{
			if (property == null)
			{
				throw new ArgumentNullException(nameof(property));
			}

			if (property.DeclaringType == null)
			{
				throw new ArgumentException($"property.DeclaringType is null", nameof(property));
			}

			name = property.Name;
			var method = property.GetGetMethod(true);
			if (method == null)
			{
				getter = NoGetter;
			}
			else
			{
				if (method.IsStatic)
				{
					throw new ArgumentException("Static properties not supported");
				}

				var dm = new DynamicMethod("__get_" + property.Name, typeof(object), new Type[] { typeof(object) }, property.DeclaringType!);
				var il = dm.GetILGenerator();
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Castclass, property.DeclaringType!);
				il.Emit(OpCodes.Callvirt, method);
				if (property.PropertyType.IsValueType)
				{
					il.Emit(OpCodes.Box, property.PropertyType);
				}
				il.Emit(OpCodes.Ret);
				getter = (Func<object, object>)dm.CreateDelegate(typeof(Func<object, object>));
			}
			method = property.GetSetMethod(true);
			if (method == null)
			{
				setter = NoSetter;
			}
			else
			{
				if (method.IsStatic)
				{
					throw new ArgumentException("Static properties not supported");
				}

				var dm = new DynamicMethod("__set_" + property.Name, null, new Type[] { typeof(object), typeof(object) }, property.DeclaringType!);
				var il = dm.GetILGenerator();
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Castclass, property.DeclaringType!);
				il.Emit(OpCodes.Ldarg_1);
				if (property.PropertyType.IsValueType)
				{
					il.Emit(OpCodes.Unbox_Any, property.PropertyType);
				}
				else
				{
					il.Emit(OpCodes.Castclass, property.PropertyType);
				}
				il.Emit(OpCodes.Callvirt, method);
				il.Emit(OpCodes.Ret);
				setter = (Action<object, object>)dm.CreateDelegate(typeof(Action<object, object>));
			}
		}
		/*public MemberAccessor(FieldInfo field)
		{
			if (field == null)
			{
				throw new ArgumentNullException("field");
			}

			if (field.IsStatic)
			{
				throw new ArgumentException("Static fields not supported");
			}

			name = field.Name;
			var method = new DynamicMethod("__get_" + field.Name, typeof(object), new Type[] { typeof(object) }, field.DeclaringType);
			var il = method.GetILGenerator();
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Castclass, field.DeclaringType);
			il.Emit(OpCodes.Ldfld, field);
			if (field.FieldType.IsValueType)
			{
				il.Emit(OpCodes.Box, field.FieldType);
			}
			il.Emit(OpCodes.Ret);
			getter = (Func<object, object>)method.CreateDelegate(typeof(Func<object, object>));

			method = new DynamicMethod("__set_" + field.Name, null, new Type[] { typeof(object), typeof(object) }, field.DeclaringType);
			il = method.GetILGenerator();
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Castclass, field.DeclaringType);
			il.Emit(OpCodes.Ldarg_1);
			if (field.FieldType.IsValueType)
			{
				il.Emit(OpCodes.Unbox_Any, field.FieldType);
			}
			else
			{
				il.Emit(OpCodes.Castclass, field.FieldType);
			}
			il.Emit(OpCodes.Stfld, field);
			il.Emit(OpCodes.Ret);
			setter = (Action<object, object>)method.CreateDelegate(typeof(Action<object, object>));
		}*/
	}
}